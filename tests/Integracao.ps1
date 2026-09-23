param([string]$BaseUrl = 'http://localhost:5152')
$ErrorActionPreference='Stop'
$sessao=New-Object Microsoft.PowerShell.Commands.WebRequestSession
function Obter([string]$rota) { Invoke-WebRequest ($BaseUrl+$rota) -WebSession $sessao -UseBasicParsing }
function Enviar([string]$rota,[hashtable]$dados,[string]$origem) {
 $pagina=Obter $origem
 $dados['__RequestVerificationToken']=[regex]::Match($pagina.Content,'name="__RequestVerificationToken" type="hidden" value="([^"]+)"').Groups[1].Value
 Invoke-WebRequest ($BaseUrl+$rota) -Method Post -WebSession $sessao -Body $dados -UseBasicParsing
}
function Verificar([bool]$condicao,[string]$descricao){if(!$condicao){throw "FALHA: $descricao"};Write-Output "OK: $descricao"}
$entrada=Enviar '/Login' @{Email='admin@admin.com';Senha='admin'} '/Login'
Verificar ($entrada.Content.Contains('Seus sonhos t')) 'Login inicial e dashboard'
foreach($rota in @('/Dashboard','/MinhaVidaFinanceira','/Receitas','/Receitas/Editar','/Despesas','/Despesas/Editar','/Dividas','/Dividas/Editar','/Objetivos','/Objetivos/Novo','/Contribuicoes','/Calendario','/Relatorios','/Simulador','/Perfil','/Configuracoes','/Notificacoes')) {
 $pagina=Obter $rota;Verificar ($pagina.StatusCode -eq 200) "Rota $rota"
}
$hoje=Get-Date
$dados=@{Nome='Verificacao automatica';Descricao='Dados sinteticos de teste';Categoria='Viagem';Icone='fa-plane';ValorObjetivo='5.000,00';ValorInicial='1.000,00';DataInicio=$hoje.AddMonths(-1).ToString('yyyy-MM-dd');DataLimite=$hoje.AddMonths(9).ToString('yyyy-MM-dd');Frequencia='Mensal';Prioridade='2';Status='Ativo'}
$detalhes=Enviar '/Objetivos/Salvar' $dados '/Objetivos/Novo'
$id=[regex]::Match($detalhes.Content,'/Objetivos/Contribuicao/(\d+)').Groups[1].Value
Verificar (![string]::IsNullOrEmpty($id)) 'Criar objetivo com valores brasileiros'
$deposito=Enviar '/Objetivos/Contribuicao' @{ObjetivoId=$id;Valor='250,00';Data=$hoje.ToString('yyyy-MM-dd');Observacao='Teste de recalculo'} "/Objetivos/Contribuicao/$id"
Verificar ($deposito.Content.Contains('416,67')) 'Contribuicao de 250 recalcula parcela para 416,67'
Verificar ($deposito.Content.Contains('1.250,00')) 'Saldo real atualizado para 1.250'
$conclusao=Enviar '/Objetivos/Contribuicao' @{ObjetivoId=$id;Valor='3.750,00';Data=$hoje.ToString('yyyy-MM-dd');Observacao='Teste de conclusao'} "/Objetivos/Contribuicao/$id"
Verificar ($conclusao.Content.Contains('Objetivo alcan')) 'Conclusao automatica ao atingir o valor'
foreach($tipo in @('Receitas','Despesas')) {
 $lista=Enviar "/$tipo/Salvar" @{Descricao="Teste $tipo";Categoria='Outros';Valor='1.234,56';Periodicidade='Mensal';Data=$hoje.ToString('yyyy-MM-dd');Ativo='true';Fixa='true'} "/$tipo/Editar"
 Verificar ($lista.Content.Contains('1.234,56')) "Salvar $tipo com centavos"
}
$divida=Enviar '/Dividas/Salvar' @{Descricao='Teste divida';Credor='Teste';ValorTotal='1.200,00';ValorParcela='100,00';QuantidadeParcelas='12';ParcelasPagas='2';DataInicio=$hoje.ToString('yyyy-MM-dd')} '/Dividas/Editar'
Verificar ($divida.Content.Contains('1.000,00')) 'Divida calcula saldo restante'
$simulacao=Enviar '/Objetivos/Simular' @{Nome='Teste';ValorObjetivo='1.200,00';ValorInicial='0,00';DataInicio=$hoje.ToString('yyyy-MM-dd');DataLimite=$hoje.AddYears(1).ToString('yyyy-MM-dd');Frequencia='Mensal';Categoria='Outro';Icone='fa-bullseye';Prioridade='2';Status='Ativo'} '/Simulador'
Verificar (($simulacao.Content|ConvertFrom-Json).mensal -eq 100) 'Simulacao executada no backend'
$inverso=Enviar '/Simulador/Inverso' @{valorMensal='300,00';meses='12'} '/Simulador'
Verificar ($inverso.Content.Contains('3.600,00')) 'Simulacao inversa'
try{Invoke-WebRequest ($BaseUrl+'/Objetivos/Excluir/'+$id) -Method Post -WebSession $sessao -UseBasicParsing | Out-Null;throw 'CSRF aceito'}catch{Verificar ($_.Exception.Response.StatusCode.value__ -eq 400) 'POST sem antiforgery rejeitado'}
$null=Enviar '/Sair' @{} '/Dashboard'
$email='teste-'+[guid]::NewGuid().ToString('N')+'@metamais.local'
$null=Enviar '/Cadastro' @{Nome='Usuario de teste';Email=$email;Senha='TesteSeguro123';ConfirmacaoSenha='TesteSeguro123'} '/Cadastro'
$null=Enviar '/Login' @{Email=$email;Senha='TesteSeguro123'} '/Login'
try{Obter "/Objetivos/Detalhes/$id"|Out-Null;throw 'Acesso indevido'}catch{Verificar ($_.Exception.Response.StatusCode.value__ -eq 404) 'Isolamento de objetivos entre usuarios'}
Write-Output 'Integracao concluida no banco separado metamais_testes.'
