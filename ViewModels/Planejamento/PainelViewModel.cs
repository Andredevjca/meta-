using System.ComponentModel.DataAnnotations;
using MetaMais.Models;
namespace MetaMais.ViewModels;

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
