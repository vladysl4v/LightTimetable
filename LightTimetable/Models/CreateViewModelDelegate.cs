using LightTimetable.ViewModels.Pages;


namespace LightTimetable.Models
{
    public delegate TViewModel CreateViewModel<out TViewModel>() where TViewModel : PageViewModelBase;
}
