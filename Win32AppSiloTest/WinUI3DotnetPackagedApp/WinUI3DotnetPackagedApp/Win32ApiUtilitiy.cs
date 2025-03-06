using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;
using Windows.Win32;
using Windows.Win32.Security;

namespace WinUI3DotnetPackagedApp;

static class Win32ApiUtilitiy
{
    public static unsafe async Task<string> PickFileViaDialog()
    {
        PInvoke.CoCreateInstance(typeof(FileOpenDialog).GUID, null, CLSCTX.CLSCTX_INPROC_SERVER, out IFileOpenDialog picker).ThrowOnFailure();
        //if (GetTokenIsAppContainer())
        //{
        //    picker.SetOptions(FILEOPENDIALOGOPTIONS.FOS_HIDEMRUPLACES | FILEOPENDIALOGOPTIONS.FOS_HIDEPINNEDPLACES);
        //    var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        //    IShellItem initialFolder = ComFolderPickerCommand.GetShellItemForPath(folder.Path);
        //    if (initialFolder != null)
        //    {
        //        picker.SetFolder(initialFolder);
        //        picker.SetDefaultFolder(initialFolder);
        //    }
        //}
        picker.Show(default);
        picker.GetResult(out var item);
        item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out var pathStr);
        var result = pathStr.ToString();
        Marshal.FreeCoTaskMem((nint)pathStr.Value);
        return result;
    }
    public unsafe static bool GetTokenIsElevated()
    {
        // from https://learn.microsoft.com/en-us/windows/win32/secauthz/appcontainer-for-legacy-applications-#c-pinvoke
        UInt32 TOKEN_QUERY = 0x0008;

        var ph = PInvoke.GetCurrentProcess();
        var safeHandle = new SafeProcessHandle((nint)ph.Value, ownsHandle: false);
        if (!PInvoke.OpenProcessToken(
            safeHandle,
            TOKEN_ACCESS_MASK.TOKEN_QUERY,
            out var tokenHandle))
        {
            throw new Exception("Win32 API Exception");
            // Handle the error.
        }

        uint result;
        uint TokenIsAppContainer = 29;
        uint tokenInformationLength = sizeof(uint);

        if (!PInvoke.GetTokenInformation(
            tokenHandle,
            TOKEN_INFORMATION_CLASS.TokenElevation,
            &result,
            tokenInformationLength,
            out tokenInformationLength))
        {
            throw new Exception("Win32 API Exception");
            // Handle the error.
        }

        return result != 0;
    }
    public unsafe static bool GetTokenIsAppContainer()
    {
        // from https://learn.microsoft.com/en-us/windows/win32/secauthz/appcontainer-for-legacy-applications-#c-pinvoke
        UInt32 TOKEN_QUERY = 0x0008;

        var ph = PInvoke.GetCurrentProcess();
        var safeHandle = new SafeProcessHandle((nint)ph.Value, ownsHandle: false);
        if (!PInvoke.OpenProcessToken(
            safeHandle,
            TOKEN_ACCESS_MASK.TOKEN_QUERY,
            out var tokenHandle))
        {
            throw new Exception("Win32 API Exception");
            // Handle the error.
        }

        uint isAppContainer;
        uint TokenIsAppContainer = 29;
        uint tokenInformationLength = sizeof(uint);

        if (!PInvoke.GetTokenInformation(
            tokenHandle,
            TOKEN_INFORMATION_CLASS.TokenIsAppContainer,
            &isAppContainer,
            tokenInformationLength,
            out tokenInformationLength))
        {
            throw new Exception("Win32 API Exception");
            // Handle the error.
        }

        return isAppContainer != 0;
    }
}
