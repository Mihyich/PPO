using System.Threading.Tasks;

namespace MetroGidDesktop.Services.Interfaces;

public interface IDialogService
{
    Task<bool?> ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : class;
}