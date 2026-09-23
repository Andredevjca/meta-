namespace MetaMais.ViewModels;

public record RelatoriosViewModel(PainelViewModel Painel, List<IndicadorRelatorioViewModel> Evolucao, List<IndicadorRelatorioViewModel> Categorias);
