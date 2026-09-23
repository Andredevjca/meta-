using MetaMais.Interfaces.Services;
using MetaMais.Interfaces.Repositories;
using Dapper;
using MetaMais.Models;
using MetaMais.Services;
using MetaMais.ViewModels;
namespace MetaMais.Repositories;
public class FinanceiroRepository(Banco banco, ICalculadoraObjetivoService calculadora) : IFinanceiroRepository
{
 private const string ConsultaObjetivos = "SELECT o.*, COALESCE(c.Total,0) TotalContribuido, c.Ultima UltimaContribuicao FROM objetivos o LEFT JOIN (SELECT ObjetivoId,SUM(Valor) Total,MAX(Data) Ultima FROM contribuicoes_objetivos GROUP BY ObjetivoId) c ON c.ObjetivoId=o.Id WHERE o.UsuarioId=@UsuarioId";
 public async Task<Usuario?> UsuarioAsync(string email) { await using var c=banco.Abrir(); return await c.QuerySingleOrDefaultAsync<Usuario>("SELECT * FROM usuarios WHERE Email=@email",new { email }); }
 public async Task<Usuario?> UsuarioAsync(int id) { await using var c=banco.Abrir(); return await c.QuerySingleOrDefaultAsync<Usuario>("SELECT * FROM usuarios WHERE Id=@id",new { id }); }
 public async Task CriarUsuarioAsync(Usuario usuario) {
  await using var c=banco.Abrir(); await c.OpenAsync(); await using var t=await c.BeginTransactionAsync();
  var id=await c.ExecuteScalarAsync<int>("INSERT INTO usuarios (Nome,Email,SenhaHash) VALUES (@Nome,@Email,@SenhaHash); SELECT LAST_INSERT_ID();",usuario,t);
  await c.ExecuteAsync("INSERT INTO configuracoes_usuario (UsuarioId) VALUES (@id)",new {id},t); await t.CommitAsync();
 }
 public async Task PerfilAsync(Usuario u, string? hash) { await using var c=banco.Abrir(); await c.ExecuteAsync("UPDATE usuarios SET Nome=@Nome,Telefone=@Telefone,DataNascimento=@DataNascimento,SenhaHash=COALESCE(@hash,SenhaHash),TrocarSenha=IF(@hash IS NULL,TrocarSenha,0) WHERE Id=@Id",new {u.Nome,u.Telefone,u.DataNascimento,u.Id,hash}); }
 public async Task<List<Objetivo>> ObjetivosAsync(int usuarioId) { await using var c=banco.Abrir(); return (await c.QueryAsync<Objetivo>(ConsultaObjetivos+" ORDER BY o.Prioridade,o.DataLimite",new {usuarioId})).ToList(); }
 public async Task<Objetivo?> ObjetivoAsync(int id,int usuarioId) { await using var c=banco.Abrir(); return await c.QuerySingleOrDefaultAsync<Objetivo>(ConsultaObjetivos+" AND o.Id=@id",new {id,usuarioId}); }
 public async Task<int> SalvarObjetivoAsync(Objetivo o)
 {
  await using var c=banco.Abrir(); await c.OpenAsync(); await using var t=await c.BeginTransactionAsync();
  if (o.Id==0) o.Id=await c.ExecuteScalarAsync<int>("INSERT INTO objetivos (UsuarioId,Nome,Descricao,Categoria,Icone,ValorObjetivo,ValorInicial,DataInicio,DataLimite,Frequencia,Status,Prioridade) VALUES (@UsuarioId,@Nome,@Descricao,@Categoria,@Icone,@ValorObjetivo,@ValorInicial,@DataInicio,@DataLimite,@Frequencia,@Status,@Prioridade); SELECT LAST_INSERT_ID();",o,t);
  else {
   var existente=await c.QuerySingleOrDefaultAsync<Objetivo>("SELECT * FROM objetivos WHERE Id=@Id AND UsuarioId=@UsuarioId FOR UPDATE",o,t);
   if (existente is null) throw new InvalidOperationException("Objetivo não encontrado.");
   await c.ExecuteAsync("UPDATE objetivos SET Nome=@Nome,Descricao=@Descricao,Categoria=@Categoria,Icone=@Icone,ValorObjetivo=@ValorObjetivo,ValorInicial=@ValorInicial,DataInicio=@DataInicio,DataLimite=@DataLimite,Frequencia=@Frequencia,Status=@Status,Prioridade=@Prioridade WHERE Id=@Id AND UsuarioId=@UsuarioId",o,t);
  }
  await c.ExecuteAsync("UPDATE objetivos SET Status=IF(ValorInicial+COALESCE((SELECT SUM(Valor) FROM contribuicoes_objetivos WHERE ObjetivoId=@Id),0)>=ValorObjetivo,'Concluído',IF(Status='Concluído','Ativo',Status)), DataConclusao=IF(ValorInicial+COALESCE((SELECT SUM(Valor) FROM contribuicoes_objetivos WHERE ObjetivoId=@Id),0)>=ValorObjetivo,COALESCE(DataConclusao,CURDATE()),NULL) WHERE Id=@Id AND UsuarioId=@UsuarioId",o,t);
  await c.ExecuteAsync("INSERT INTO historico_objetivos (ObjetivoId,Descricao) VALUES (@Id,@descricao)",new {o.Id,descricao=$"Planejamento salvo: {o.Nome}; objetivo {o.ValorObjetivo:C}; inicial {o.ValorInicial:C}; prazo {o.DataLimite:dd/MM/yyyy}; frequência {o.Frequencia}; status {o.Status}."},t);
  await t.CommitAsync(); return o.Id;
 }
 public async Task<string> ContribuirAsync(Contribuicao deposito,int usuarioId)
 {
  await using var c=banco.Abrir(); await c.OpenAsync(); await using var t=await c.BeginTransactionAsync();
  var o=await c.QuerySingleOrDefaultAsync<Objetivo>("SELECT * FROM objetivos WHERE Id=@ObjetivoId AND UsuarioId=@usuarioId FOR UPDATE",new {deposito.ObjetivoId,usuarioId},t);
  if (o is null || o.Status!="Ativo") throw new InvalidOperationException("Selecione um objetivo ativo.");
  if (deposito.Data.Date>DateTime.Today || deposito.Data.Date<o.DataInicio.Date) throw new InvalidOperationException("A data deve estar entre o início do objetivo e hoje.");
  o.TotalContribuido=await c.ExecuteScalarAsync<decimal>("SELECT COALESCE(SUM(Valor),0) FROM contribuicoes_objetivos WHERE ObjetivoId=@Id",o,t);
  var antes=calculadora.Calcular(o);
  await c.ExecuteAsync("INSERT INTO contribuicoes_objetivos (ObjetivoId,Valor,Data,Observacao) VALUES (@ObjetivoId,@Valor,@Data,@Observacao)",deposito,t);
  o.TotalContribuido+=deposito.Valor; o.UltimaContribuicao=deposito.Data;
  var depois=calculadora.Calcular(o);
  var diferenca=deposito.Valor-antes.PorPeriodo;
  var mensagem=depois.Restante==0 ? "Objetivo alcançado! Você conseguiu conquistar sua meta." : diferenca==0 ? "Meta cumprida." : $"Você guardou {Math.Abs(diferenca):C} {(diferenca>0?"acima":"abaixo")} do planejado. Próximas contribuições: {depois.PorPeriodo:C} ({o.Frequencia.ToLower()}).";
  if (depois.Restante==0) await c.ExecuteAsync("UPDATE objetivos SET Status='Concluído',DataConclusao=@Data WHERE Id=@ObjetivoId",deposito,t);
  await c.ExecuteAsync("INSERT INTO historico_objetivos (ObjetivoId,Descricao) VALUES (@ObjetivoId,@descricao)",new {deposito.ObjetivoId,descricao=$"Contribuição de {deposito.Valor:C}. Saldo: {o.ValorAcumulado:C}. {mensagem}"},t);
  await c.ExecuteAsync("INSERT INTO notificacoes (UsuarioId,Mensagem) VALUES (@usuarioId,@mensagem)",new {usuarioId,mensagem},t);
  await t.CommitAsync(); return mensagem;
 }
 public async Task<List<Contribuicao>> ContribuicoesAsync(int usuarioId,int? objetivoId=null) { await using var c=banco.Abrir(); return (await c.QueryAsync<Contribuicao>("SELECT c.*,o.Nome NomeObjetivo FROM contribuicoes_objetivos c JOIN objetivos o ON o.Id=c.ObjetivoId WHERE o.UsuarioId=@usuarioId AND (@objetivoId IS NULL OR o.Id=@objetivoId) ORDER BY c.Data DESC,c.Id DESC",new {usuarioId,objetivoId})).ToList(); }
 public async Task<List<HistoricoObjetivo>> HistoricoAsync(int id,int usuarioId) { await using var c=banco.Abrir(); return (await c.QueryAsync<HistoricoObjetivo>("SELECT h.* FROM historico_objetivos h JOIN objetivos o ON o.Id=h.ObjetivoId WHERE o.Id=@id AND o.UsuarioId=@usuarioId ORDER BY h.Id DESC",new {id,usuarioId})).ToList(); }
 private static string Tabela(string tipo) => tipo switch { "Receitas"=>"receitas", "Despesas"=>"despesas", "Dividas"=>"dividas", "Objetivos"=>"objetivos", _=>throw new ArgumentException("Tipo inválido") };
 public async Task<List<Lancamento>> LancamentosAsync(string tipo,int usuarioId) { await using var c=banco.Abrir(); return (await c.QueryAsync<Lancamento>($"SELECT * FROM {Tabela(tipo)} WHERE UsuarioId=@usuarioId ORDER BY Data DESC",new {usuarioId})).ToList(); }
 public async Task SalvarLancamentoAsync(string tipo,Lancamento l) { await using var c=banco.Abrir(); await c.ExecuteAsync(l.Id==0 ? $"INSERT INTO {Tabela(tipo)} (UsuarioId,Descricao,Categoria,Valor,Periodicidade,Data,Ativo,Fixa) VALUES (@UsuarioId,@Descricao,@Categoria,@Valor,@Periodicidade,@Data,@Ativo,@Fixa)" : $"UPDATE {Tabela(tipo)} SET Descricao=@Descricao,Categoria=@Categoria,Valor=@Valor,Periodicidade=@Periodicidade,Data=@Data,Ativo=@Ativo,Fixa=@Fixa WHERE Id=@Id AND UsuarioId=@UsuarioId",l); }
 public async Task<List<Divida>> DividasAsync(int usuarioId) { await using var c=banco.Abrir(); return (await c.QueryAsync<Divida>("SELECT * FROM dividas WHERE UsuarioId=@usuarioId ORDER BY DataInicio",new {usuarioId})).ToList(); }
 public async Task SalvarDividaAsync(Divida d) { await using var c=banco.Abrir(); await c.ExecuteAsync(d.Id==0 ? "INSERT INTO dividas (UsuarioId,Descricao,Credor,ValorTotal,ValorParcela,QuantidadeParcelas,ParcelasPagas,DataInicio) VALUES (@UsuarioId,@Descricao,@Credor,@ValorTotal,@ValorParcela,@QuantidadeParcelas,@ParcelasPagas,@DataInicio)" : "UPDATE dividas SET Descricao=@Descricao,Credor=@Credor,ValorTotal=@ValorTotal,ValorParcela=@ValorParcela,QuantidadeParcelas=@QuantidadeParcelas,ParcelasPagas=@ParcelasPagas,DataInicio=@DataInicio WHERE Id=@Id AND UsuarioId=@UsuarioId",d); }
 public async Task ExcluirAsync(string tipo,int id,int usuarioId) { await using var c=banco.Abrir(); await c.ExecuteAsync($"DELETE FROM {Tabela(tipo)} WHERE Id=@id AND UsuarioId=@usuarioId",new {id,usuarioId}); }
 public async Task<List<string>> NotificacoesAsync(int usuarioId) { await using var c=banco.Abrir(); return (await c.QueryAsync<string>("SELECT Mensagem FROM notificacoes WHERE UsuarioId=@usuarioId AND Lida=0 ORDER BY Id DESC LIMIT 20",new {usuarioId})).ToList(); }
 public async Task LerNotificacoesAsync(int usuarioId) { await using var c=banco.Abrir(); await c.ExecuteAsync("UPDATE notificacoes SET Lida=1 WHERE UsuarioId=@usuarioId",new {usuarioId}); }
}
