using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class HomeViewModel: ObservableObject
{
    [ObservableProperty]
    private string _uploadedScale;

    [ObservableProperty]
    private string _uploadedZip;

    public HomeViewModel() 
    {
        UploadedScale = "None";
        UploadedZip = "None";
    }
}