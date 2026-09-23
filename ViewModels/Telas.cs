using System.ComponentModel.DataAnnotations;
using MetaMais.Models;
namespace MetaMais.ViewModels;
public class LoginViewModel
{
 [Required, EmailAddress] public string Email { get; set; } = "";
 [Required, StringLength(72), DataType(DataType.Password)] public string Senha { get; set; } = "";
 public bool LembrarMe { get; set; }
}
public class CadastroViewModel : LoginViewModel
{
 [Required, StringLength(100)] public string Nome { get; set; } = "";
 [Compare(nameof(Senha), ErrorMessage="As senhas não coincidem.")] public string ConfirmacaoSenha { get; set; } = "";
}
public record CalculoObjetivo(decimal Restante, decimal Percentual, int Periodos, decimal PorPeriodo, decimal Mensal, decimal Quinzenal, decimal Semanal, DateTime? ProximoCheckin, bool Atrasado, DateTime? Previsao);
public record ObjetivoPlanejado(Objetivo Objetivo, CalculoObjetivo Calculo);
public class PainelViewModel
{
 public List<ObjetivoPlanejado> Objetivos { get; set; } = [];
 public List<Lancamento> Receitas { get; set; } = [];
 public List<Lancamento> Despesas { get; set; } = [];
 public List<Divida> Dividas { get; set; } = [];
 public List<Contribuicao> Contribuicoes { get; set; } = [];
 public decimal ReceitaMensal { get; set; }
 public decimal DespesaMensal { get; set; }
 public decimal ParcelasMensais { get; set; }
 public decimal Capacidade => ReceitaMensal - DespesaMensal - ParcelasMensais;
 public decimal Necessario => Objetivos.Where(x => x.Objetivo.Status == "Ativo").Sum(x => x.Calculo.Mensal);
 public decimal Margem => Capacidade - Necessario;
}
public record DetalheObjetivoViewModel(ObjetivoPlanejado Planejado, List<Contribuicao> Contribuicoes, List<HistoricoObjetivo> Historico);
