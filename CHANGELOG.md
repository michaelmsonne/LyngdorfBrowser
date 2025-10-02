# Changelog

## [1.0.0.7] - 02-10-2025
Updates:
- Update 3. party packets there is used by the application
    - CefSharp.WinForms.139.0.280 -> CefSharp.WinForms.140.1.140
    - CefSharp.Common.139.0.280 -> CefSharp.Common.140.1.140
    - cef.redist.x86.139.0.280 -> cef.redist.x86.140.1.140
    - cef.redist.x64.139.0.280 -> cef.redist.x64.140.1.140

Added:
   - Add connection status indicator to MainForm - Introduced a `StatusStrip` (`connectionIndicator`) and a `Label` (`statusLabel`) to visually display the application's connection status.
   - Enhanced `CheckConnection`, `InitializeChromium`, and `TryInitializeChromiumWithArgument` to provide real-time feedback.
   - Updated `MainForm_FormClosing` to display shutdown status.
   - Ensured thread-safe UI updates using `Invoke` where necessary.
   - Improved error handling to display appropriate status messages.

## [1.0.0.6] - 18-03-2024
Updates:
- Update 3. party packets there is used by the application
    - CefSharp.WinForms.119.4.30 -> CefSharp.WinForms.122.1.120
    - CefSharp.Common.119.4.30 -> CefSharp.Common.122.1.120
    - cef.redist.x86.119.4.3 -> cef.redist.x86.120.2.7
    - cef.redist.x64.119.4.3 -> cef.redist.x64.120.2.7
Installing:
    - chromiumembeddedframework.runtime.win-x64.122.1.12
    - chromiumembeddedframework.runtime.win-x86.122.1.12 

## [1.0.0.5] - 11-12-2023
Updates:
- Clode cleanup/optimization
- Add error check for IPMacMapper class
- Update 3. party packets there is used by the application
    - cef.redist.x64 version 119.1.2 to 119.4.3
    - cef.redist.x86 version 119.1.2 to 119.4.3
    - CefSharp.Common version 119.1.20 to 119.4.30
    - CefSharp.WinForms version 119.1.20 to 119.4.30

## [1.0.0.4] - 18-11-2023
Updates:
- Clode cleanup/optimization
- Update 3. party packets there is used by the application
    - cef.redist.x64 from 117.2.4 to 119.1.2
    - CefSharp.WinForms from 117.2.40 to 119.1.20
    - cef.redist.x86 from 117.2.4 to 119.1.2
    - CefSharp.Common from 117.2.40 to 119.1.20

## [1.0.0.3] - 24-10-2023
Added:
  - Basic log function to the application

Updates:
- Clode cleanup
- Update 3. party packets there is used by the application
    - CefSharp.WinForms.116.0.230 -> 117.2.40
    - CefSharp.Common.116.0.230 -> 117.2.40
    - cef.redist.x86.116.0.23 -> 117.2.4
    - cef.redist.x64.116.0.23 -> 117.2.4

## [1.0.0.2] - 28-09-2023
Updates:
- Updates to IpMacMapper class
- Small changes to main code
Update 3. party packets there is used by the application
    - CefSharp.WinForms.114.2.100 -> CefSharp.WinForms.116.0.230
    - CefSharp.Common.114.2.100 -> CefSharp.Common.116.0.230
    - cef.redist.x86.114.2.10 -> cef.redist.x86.116.0.23
    - cef.redist.x64.114.2.10 -> cef.redist.x64.116.0.23

## [1.0.0.1] - 17-6-2023
- Update 3. party packets there is used by the application
    - cef.redist from 113.1.4 to 114.2.10
    - CefSharp.Common from 113.1.40 to 114.2.100
    - CefSharp.WinForms from 113.1.40 to 114.2.100

## [1.0.0.0] - 14-5-2023
- Initial release