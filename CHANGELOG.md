## [Eyes.Sdk.Core vNext patch]
### Fixed
- NuGets that depends on this one were unable to install.

## [Eyes.Appium vNext patch]
### Fixed
- NuGet was unable to install.

## [Eyes.Images vNext patch]
### Fixed
- NuGet was unable to install.

## [Eyes.CodedUI vNext patch]
### Fixed
- NuGet was unable to install.

## [Eyes.Selenium vNext patch]
### Fixed
- NuGet was unable to install.

## [Eyes.LeanFT vNext patch]
### Fixed
- NuGet was unable to install.

## [Eyes.Windows vNext patch]
### Fixed
- NuGet was unable to install.

## [Eyes.Appium 4.13] - 2020-10-08
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Images 2.17] - 2020-10-08
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.CodedUI 2.17] - 2020-10-08
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Selenium 2.35] - 2020-10-08
### Updated
- Added log prints.
### Fixed
- Fixed Simple Coded Regions visible element rectangle computation. [Trello 2143](https://trello.com/c/Xl2uQTyh)
- Fixed the way DOM Capture and DOM Snapshot scripts are chosen. [Trello 2196](https://trello.com/c/nvHzS0Ba)

## [Eyes.LeanFT 2.19] - 2020-10-08
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Windows 2.18] - 2020-10-08
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Sdk.Core 2.35] - 2020-10-08
### Updated
- Added log prints.
- Updated [HtmlAgilityPack](https://www.nuget.org/packages/HtmlAgilityPack/1.11.24) dependency.
### Added
- Support for `IosVersion` in `IosDeviceInfo`. [Trello 2187](https://trello.com/c/25AjSV6V)
### Fixed
- Fixed the way DOM Capture and DOM Snapshot scripts are chosen. [Trello 2196](https://trello.com/c/nvHzS0Ba)
- Fixed collection of resources from CSS files. [Trello 2156](https://trello.com/c/mSS6mLxP)

## [Eyes.Appium 4.12] - 2020-09-03
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Images 2.16] - 2020-09-03
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.CodedUI 2.16] - 2020-09-03
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Selenium 2.34] - 2020-09-03
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.LeanFT 2.18] - 2020-09-03
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Windows 2.17] - 2020-09-03
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Sdk.Core 2.34] - 2020-09-03
### Fixed
- Correct Agent Id is now returned from `FullAgentId`. [Trello 2137](https://trello.com/c/pYl5ZHtR)

## [Eyes.Appium 4.11] - 2020-09-02
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Images 2.15] - 2020-09-02
### Updated
- Added `replaceLast` arguments with default value `false` to some methods. [Trello 1768](https://trello.com/c/AYWylsfL)

## [Eyes.CodedUI 2.15] - 2020-09-02
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Selenium 2.33] - 2020-09-02
### Fixed
- When checking full element, don't attempt moving it if it's already in viewport. [Trello 2097](https://trello.com/c/JWjjtoc8)
### Added
- Skip list for DOM snapshot [Trello 1974](https://trello.com/c/44xq8dze)

## [Eyes.LeanFT 2.17] - 2020-09-02
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Windows 2.16] - 2020-09-02
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Sdk.Core 2.33] - 2020-09-02
### Added
- Skip list for DOM snapshot [Trello 1974](https://trello.com/c/44xq8dze)

## [Eyes.Qtp 1.14] - 2020-08-27
### Fixed
- Fixed duplicated steps due to updated retry mechanism. [Trello 1768](https://trello.com/c/AYWylsfL)

## [Eyes.Appium 4.10] - 2020-08-23
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Images 2.14] - 2020-08-23
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.CodedUI 2.14] - 2020-08-23
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Selenium 2.32] - 2020-08-23
### Updated
- Coerced stitched image size to the maximum acceptible by the server. [Trello 1991](https://trello.com/c/2iCNfoI7)
- Try to send correct iOS device size when rendering fails. [Trello 2006](https://trello.com/c/a6l6gTf9)
- DOM snapshot script to 4.0.5. [Trello 2006](https://trello.com/c/a6l6gTf9)
- When render fails, the correct useragent will be sent in the test results. [Trello 2086](https://trello.com/c/RLOmjJLT)
- When render fails, the correct device size will be sent in the test results. [Trello 2087](https://trello.com/c/AQ0upINc)
### Added
- UltrafastGrid now supports `VisualGridOptions` in both Configuration and in fluent API. [Trello 2089](https://trello.com/c/d4zggQes)
- Missing Fluent APIs found by a new test. [Trello 2107](https://trello.com/c/jotTpM5o)
### Fixed
- When checking element, don't attempt moving it if it's already in viewport. [Trello 2097](https://trello.com/c/JWjjtoc8)

## [Eyes.LeanFT 2.16] - 2020-08-23
### Updated
- Coerced stitched image size to the maximum acceptible by the server. [Trello 1991](https://trello.com/c/2iCNfoI7)

## [Eyes.Windows 2.15] - 2020-08-23
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Sdk.Core 2.32] - 2020-08-23
### Updated
- Coerced stitched image size to the maximum acceptible by the server. [Trello 1991](https://trello.com/c/2iCNfoI7)
### Added
- UltrafastGrid now supports `VisualGridOptions` in both Configuration and in fluent API. [Trello 2089](https://trello.com/c/d4zggQes)

## [Eyes.Appium 4.9] - 2020-07-30
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Images 2.13] - 2020-07-30
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.CodedUI 2.13] - 2020-07-30
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Selenium 2.31] - 2020-07-30
### Fixed
- Use `ScreenOrientation.Portrait` as a default value for `IosDeviceInfo` constructor, instead of `null`. [Trello 1999](https://trello.com/c/oHtG44v7)
- Coded regions now clipped to their visible part. [Trello 1945](https://trello.com/c/2en5OsIU)
### Updated
- The default scroll root element is now the bigger one between "body" and "html" instead of only "html". [Trello 1992](https://trello.com/c/awB5BJhy)
- Added Visual-Viewport support to UFG. [Trello 1957](https://trello.com/c/jWvdBwex)

## [Eyes.LeanFT 2.15] - 2020-07-30
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Windows 2.14] - 2020-07-30
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Sdk.Core 2.31] - 2020-07-30
### Fixed
- Use `ScreenOrientation.Portrait` as a default value for `IosDeviceInfo` constructor, instead of `null`. [Trello 1999](https://trello.com/c/oHtG44v7)
### Updated
- Added Visual-Viewport support to UFG. [Trello 1957](https://trello.com/c/jWvdBwex)

## [Eyes.Appium 4.8] - 2020-07-09
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Images 2.12] - 2020-07-09
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.CodedUI 2.12] - 2020-07-09
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Selenium 2.30] - 2020-07-09
### Fixed
- Wrong position of page on retry in certain cases. [Trello 1828](https://trello.com/c/eGjuq1mG)

## [Eyes.LeanFT 2.14] - 2020-07-09
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Windows 2.13] - 2020-07-09
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Sdk.Core 2.30] - 2020-07-09
### Updated
- Added missing `StitchingService` URI field in `RenderRequest`. [Trello 1988](https://trello.com/c/Yr6EsUlL)
### Fixed
- Wrong position of page on retry in certain cases. [Trello 1828](https://trello.com/c/eGjuq1mG)

## [Eyes.CodedUI 2.11] - 2020-07-05
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Images 2.11] - 2020-07-05
### Added
- Added overload `Applitools.Images.Eyes.Check(ICheckSettings checkSettings)`.

## [Eyes.Appium 4.7] - 2020-07-05
### Added
- Element and Selector based coded regions for native apps. [Trello 1897](https://trello.com/c/EG7LU8cl)

## [Eyes.Selenium 2.29] - 2020-07-05
### Fixed
- Calling `Eyes.Open` with a different viewport size after `Eyes.Close` fails to set the viewport. [Trello 1923](https://trello.com/c/6RmlJbxk)
- Instantiating `Eyes(Uri serverUrl)` caused `NullReferenceException` to be thrown. [Trello 1975](https://trello.com/c/sdvIjyqs)

## [Eyes.LeanFT 2.13] - 2020-07-05
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Windows 2.12] - 2020-07-05
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Sdk.Core 2.29] - 2020-07-05
### Updated
- Suppress sending identical images to the server. [Trello 1866](https://trello.com/c/KyxkI6Bu)
### Added
- Added `ReplaceLast` to `ICheckSettings`.
### Fixed
- Removed `IosScreenOrientation` enum in favor of existing `ScreenOrientation` enum due to same viewports issue. [Trello 1944](https://trello.com/c/EzyG7525)
- `FileLogHandler.Open` now restarts if was perviously closed. [Trello 1958](https://trello.com/c/M6pcYPe4)

## [Eyes.Appium 4.6] - 2020-06-22
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Images 2.10] - 2020-06-22
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.CodedUI 2.10] - 2020-06-22
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Selenium 2.28] - 2020-06-22
### Fixed
- Stitching algorithm. [Trello 1668](https://trello.com/c/155X9P8x)

## [Eyes.LeanFT 2.12] - 2020-06-22
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Windows 2.11] - 2020-06-22
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Sdk.Core 2.28] - 2020-06-22
### Fixed
- Stitching algorithm. [Trello 1668](https://trello.com/c/155X9P8x)

## [Eyes.Appium 4.5] - 2020-06-11
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Images 2.9] - 2020-06-11
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.CodedUI 2.9] - 2020-06-11
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Selenium 2.27] - 2020-06-11
### Updated
- A better API for the Ultrafast Grid. [Trello 1872](https://trello.com/c/bykk2rzB)

## [Eyes.LeanFT 2.11] - 2020-06-11
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Windows 2.10] - 2020-06-11
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Sdk.Core 2.27] - 2020-06-11
### Updated
- A better API for the Ultrafast Grid. [Trello 1872](https://trello.com/c/bykk2rzB)

## [Eyes.Appium 4.4] - 2020-06-09
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Images 2.8] - 2020-06-09
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.CodedUI 2.8] - 2020-06-09
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Selenium 2.26] - 2020-06-09
### Fixed
- Ultrafast Grid iOS simulators now sends the correct device name. [Trello 1872](https://trello.com/c/bykk2rzB)

## [Eyes.LeanFT 2.10] - 2020-06-09
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Windows 2.9] - 2020-06-09
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Sdk.Core 2.26] - 2020-06-09
### Fixed
- Ultrafast Grid iOS simulators now sends the correct device name. [Trello 1872](https://trello.com/c/bykk2rzB)

## [Eyes.Appium 4.3] - 2020-06-07
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Images 2.7] - 2020-06-07
### Fixed
- Coded Regions not getting sent to server in all packages. [Trello 1823](https://trello.com/c/FT5xdO9O)

## [Eyes.CodedUI 2.7] - 2020-06-07
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Selenium 2.25] - 2020-06-07
### Added
- Ultrafast Grid now supports iOS simulators. [Trello 1872](https://trello.com/c/bykk2rzB)
### Fixed
- Closed open connections. [Trello 1863](https://trello.com/c/WwqfGgfS)
- Unsubscribe an event upon destruction of `VisualGridRunner`. [Trello 1863](https://trello.com/c/WwqfGgfS)
### Updated
- DOM Capture and Snapshot scripts [Trello 1865](https://trello.com/c/haTeCXzq)

## [Eyes.LeanFT 2.9] - 2020-06-07
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Windows 2.8] - 2020-06-07
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Sdk.Core 2.25] - 2020-06-07
### Added
- Ultrafast Grid now supports iOS simulators. [Trello 1872](https://trello.com/c/bykk2rzB)
### Fixed
- Coded Regions not getting sent to server in all packages. [Trello 1823](https://trello.com/c/FT5xdO9O)
- Closed open connections. [Trello 1863](https://trello.com/c/WwqfGgfS)
- Unsubscribe an event upon destruction of `VisualGridRunner`. [Trello 1863](https://trello.com/c/WwqfGgfS)

## [Eyes.Sdk.Core 2.24] - 2020-05-18
### Added
- Added multiple devices to `DeviceName` enum. [Trello 1751](https://trello.com/c/JOyUqzEM)
- `EyesRunner` now supports `ApiKey`, `ServerUrl` and `IsDisabled` for easier test writing. [Trello 1809](https://trello.com/c/CYFIcIZW)
- Added Accessibility support. [Trello 1767](https://trello.com/c/gq69woeK)
### Fixed
- UltraFast Grid hangs. [Trello 1772](https://trello.com/c/vsqDCHbS)

## [Eyes.Selenium 2.24] - 2020-05-18
### Added
- Added multiple devices to `DeviceName` enum. [Trello 1751](https://trello.com/c/JOyUqzEM)
- `EyesRunner` now supports `ApiKey`, `ServerUrl` and `IsDisabled` for easier test writing [Trello 1809](https://trello.com/c/CYFIcIZW)
### Fixed
- Fixed `ForceFullPageScreenshot` in Classic API. [Trello 1764](https://trello.com/c/22MAaiex)
- UltraFast Grid hangs. [Trello 1772](https://trello.com/c/vsqDCHbS)

## [Eyes.Appium 4.2] - 2020-05-18
### Added
- Added Accessibility support. [Trello 1767](https://trello.com/c/gq69woeK)

## [Eyes.CodedUI 2.6] - 2020-05-18
### Added
- Added Accessibility support. [Trello 1767](https://trello.com/c/gq69woeK)

## [Eyes.Images 2.6] - 2020-05-18
### Added
- Added Accessibility support. [Trello 1767](https://trello.com/c/gq69woeK)

## [Eyes.LeanFT 2.8] - 2020-05-18
### Added
- Added Accessibility support. [Trello 1767](https://trello.com/c/gq69woeK)

## [Eyes.Windows 2.7] - 2020-05-18
### Added
- Added Accessibility support. [Trello 1767](https://trello.com/c/gq69woeK)

## [Eyes.Selenium 2.23.4] - 2020-04-26
### Added
- UltraFast Grid now supports Edge Chromium and Edge Legacy. [Trello 1757](https://trello.com/c/LUe43aee)
### Fixed
- `InternetExplorerScreenshotImageProvider` now gets the viewport correctly. [Trello 1742](https://trello.com/c/4MAVBbAo)

## [Eyes.Sdk.Core 2.23.3] - 2020-04-26
### Added
- UltraFast Grid now supports Edge Chromium and Edge Legacy. [Trello 1757](https://trello.com/c/LUe43aee)
### Fixed
- `StartSession` (`Eyes.Open`) request now honors `isNew` flag. [Trello 1715](https://trello.com/c/DcVzWbeR)

## [Eyes.Windows 2.6.2] - 2020-04-26
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.CodedUI 2.5.2] - 2020-04-26
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Appium 4.1.2] - 2020-04-26
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Images 2.5.2] - 2020-04-26
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.LeanFT 2.7.2] - 2020-04-26
### Updated
- Match to latest Eyes.Sdk.Core

## [Eyes.Sdk.Core 2.23.2] - 2020-04-06
### Updated
- Adding agent id to all requests headers. [Trello 1697](https://trello.com/c/CzhUxOqE)

## [Eyes.Selenium 2.23.3] - 2020-04-06
### Updated
- Adding agent id to all requests headers. [Trello 1697](https://trello.com/c/CzhUxOqE)

## [Eyes.Appium 4.1.1] - 2020-04-06
### Updated
- Adding agent id to all requests headers. [Trello 1697](https://trello.com/c/CzhUxOqE)

## [Eyes.Images 2.5.1] - 2020-04-06
### Updated
- Adding agent id to all requests headers. [Trello 1697](https://trello.com/c/CzhUxOqE)

## [Eyes.LeanFT 2.7.1] - 2020-04-06
### Updated
- Adding agent id to all requests headers. [Trello 1697](https://trello.com/c/CzhUxOqE)

## [Eyes.Windows 2.6.1] - 2020-04-06
### Updated
- Adding agent id to all requests headers. [Trello 1697](https://trello.com/c/CzhUxOqE)

## [Eyes.CodedUI 2.5.1] - 2020-04-06
### Updated
- Adding agent id to all requests headers. [Trello 1697](https://trello.com/c/CzhUxOqE)

## [Eyes.Selenium 2.23.2] - 2020-03-24
### Fixed
- Call `ILogHandler.Open()` for Ultrafast Grid.

## [Eyes.Sdk.Core 2.23.1] - 2020-03-24
### Fixed
- Fixed Viewport metatag parsing. [Trello 1629](https://trello.com/c/a0AgWIWj)
- Changed the logic of  Ultrafast Grid resources download.  [Trello 1517](https://trello.com/c/8xV9CTtB)

## [Eyes.Selenium 2.23.1] - 2020-03-15
### Fixed
- Fixed `IsDisabled` wasn't always honored. [Trello 1626](https://trello.com/c/JXopsQqt)

## [Eyes.Sdk.Core 2.23] - 2020-03-08
### Fixed
- Use `NumberFormatInfo.InvariantInfo` when parsing Javascript floating point numeric results. [Trello 1576](https://trello.com/c/06BkiWxG)
- Prevent endless loop due to negative size. [Trello 1585](https://trello.com/c/a28mYhW7)
### Updated
- Added support for .NET Framework 4.5.2, 4.6.2, 4.7.2 and 4.8.
- Added support for .NET Core 3.1 (LTS).
- Added support for .NET Standard 2.1.
- Fixed misplaced coded regions in some edge cases. [Trello 1532](https://trello.com/c/fRDzNayH)
- Upload DOM directly to storage service on MatchWindow. [Trello 1592](https://trello.com/c/MXixwLnj)
- Improved `FileLogHandler`'s performance.

## [Eyes.Selenium 2.23] - 2020-03-08
### Fixed
- Update sizes and SizeAdjuster on every check due to possible URL change. [Trello 1353](https://trello.com/c/rhTs54Kb)
- Use `NumberFormatInfo.InvariantInfo` when parsing Javascript floating point numeric results. [Trello 1576](https://trello.com/c/06BkiWxG)
- Fixed misplaced coded regions in some edge cases. [Trello 1532](https://trello.com/c/fRDzNayH)
- Match to latest Eyes.Sdk.Core
### Updated
- DOM Snapshot script to version 3.3.3. [Trello 1588](https://trello.com/c/ZS0Wb1FN)
- Added support for .NET Framework 4.5.2, 4.6.2, 4.7.2 and 4.8.
- Added support for .NET Core 3.1 (LTS).
- Added support for .NET Standard 2.1.
- Upload DOM directly to storage service on MatchWindow. [Trello 1592](https://trello.com/c/MXixwLnj)

## [Eyes.Images 2.5] - 2020-03-08
### Fixed
- Clone incoming configuration. [Trello 1560](https://trello.com/c/hSTcBcvJ)
### Updated
- Added support for .NET Framework 4.5.2, 4.6.2, 4.7.2 and 4.8.
- Added support for .NET Core 3.1 (LTS).
- Added support for .NET Standard 2.1.
- Match to latest Eyes.Sdk.Core

## [Eyes.LeanFT 2.7] - 2020-03-08
### Fixed
- Clone incoming configuration. [Trello 1560](https://trello.com/c/hSTcBcvJ)
### Updated
- Added support for .NET Framework 4.5.2, 4.6.2, 4.7.2 and 4.8.
- Added support for .NET Core 3.1 (LTS).
- Match to latest Eyes.Sdk.Core

## [Eyes.Windows 2.6] - 2020-03-08
### Fixed
- Clone incoming configuration. [Trello 1560](https://trello.com/c/hSTcBcvJ)
### Updated
- Added support for .NET Framework 4.5.2, 4.6.2, 4.7.2 and 4.8.
- Added support for .NET Core 3.1 (LTS).
- Match to latest Eyes.Sdk.Core

## [Eyes.Appium 4.1] - 2020-03-08
### Fixed
- Clone incoming configuration. [Trello 1560](https://trello.com/c/hSTcBcvJ)
### Updated
- Added support for .NET Framework 4.5.2, 4.6.2, 4.7.2 and 4.8.
- Added support for .NET Core 3.1 (LTS).
- Added support for .NET Standard 2.1.
- Match to latest Eyes.Sdk.Core

## [Eyes.CodedUI 2.5] - 2020-03-08
### Updated
- Added support for .NET Framework 4.5.2, 4.6.2, 4.7.2 and 4.8.

## [Eyes.Selenium 2.22.31] - 2020-01-21
### Updated
- Visual Grid: Added older versions support for Chrome, Firefox and Safari browsers. [Trello 1479](https://trello.com/c/kwsR1zql)
### Fixed
- Fixed default match settings being incorrectly overriden by image match settings. [Trello 1495](https://trello.com/c/KEbWXavV)

## [Eyes.Sdk.Core 2.22.31] - 2020-01-21
### Updated
- NuGet package name.
- Upload images directly to storage service on MatchWindow. [Trello 1461](https://trello.com/c/1V5X9O37)
- Visual Grid: Added older versions support for Chrome, Firefox and Safari browsers. [Trello 1479](https://trello.com/c/kwsR1zql)
### Fixed
- Fixed default match settings being incorrectly overriden by image match settings. [Trello 1495](https://trello.com/c/KEbWXavV)

## [Eyes.CodedUI 2.4.4] - 2020-01-21
### Updated
- NuGet package name.
### Fixed
- Fixed default match settings being incorrectly overriden by image match settings. [Trello 1495](https://trello.com/c/KEbWXavV)

## [Eyes.LeanFT 2.6.4] - 2020-01-21
### Updated
- NuGet package name.
### Fixed
- Fixed default match settings being incorrectly overriden by image match settings. [Trello 1495](https://trello.com/c/KEbWXavV)

## [Eyes.Appium 4.0.6] - 2020-01-21
### Fixed
- Fixed default match settings being incorrectly overriden by image match settings. [Trello 1495](https://trello.com/c/KEbWXavV)

## [Eyes.Images 2.4.5] - 2020-01-21
### Fixed
- Fixed default match settings being incorrectly overriden by image match settings. [Trello 1495](https://trello.com/c/KEbWXavV)

## [Eyes.Windows 2.5.6] - 2020-01-21
### Fixed
- Fixed default match settings being incorrectly overriden by image match settings. [Trello 1495](https://trello.com/c/KEbWXavV)

## [Eyes.Appium 4.0.5] - 2019-12-29
### Updated
- Depend on Eyes.Sdk.Core instead of Eyes.Sdk.

## [Eyes.Images 2.4.4] - 2019-12-29
### Updated
- Depend on Eyes.Sdk.Core instead of Eyes.Sdk.

## [Eyes.Windows 2.5.5] - 2019-12-29
### Updated
- Depend on Eyes.Sdk.Core instead of Eyes.Sdk.

## [Eyes.CodedUI 2.4.3] - 2019-12-29
### Updated
- Depend on Eyes.Sdk.Core instead of Eyes.Sdk.

## [Eyes.LeanFT 2.6.3] - 2019-12-29
### Updated
- Depend on Eyes.Sdk.Core instead of Eyes.Sdk.

## [Eyes.Selenium 2.22.30] - 2019-12-29
### Fixed
- Mobie devices stitching now honors `<meta name='viewport' ... >` tag. [Trello 1367](https://trello.com/c/6iB0NZCG)
### Updated
- DOM Capture scripts.
- Fixed viewport computation on edge cases.

## [Eyes.Sdk.Core 2.22.30] - 2019-12-29
### Fixed
- Stitching algorithm.

## [Eyes.Selenium 2.22.29] - 2019-12-11
### Added
- .Net Core 3.0 support
- Support for iPhone 11.
### Fixed
- Fixed logic of when to use `ElementPositionProvider`.
- Validate eyes open before check.
- Fixed edge case in FrameChain.
- Fixed iOS Safari cropping.
- Fixed stitching subregions computation.

## [Eyes.Sdk 2.22.29] - 2019-12-11
### Fixed
- `MatchWindow` now also passes `ImageMatchSettings.Exact` from `DefaultMatchSettings`.
- Retry failed connections to server.
- Only store `APIKey` and `ServerUrl` once - in the configuration object.
### Added
- .Net Core 3.0 support
### Updated
- Updated `System.Drawing.Common` dependency.
### Removed
- ImageDeltaCompressor. Images are no longer compressed by delta to the previous image.

## [Eyes.Appium 4.0.4] - 2019-12-11
### Added
- .Net Core 3.0 support
### Updated
- Updated `Appium.WebDriver` dependency.

## [Eyes.LeanFT 2.6.2] - 2019-12-11
### Added
- .Net Core 3.0 support

## [Eyes.Images 2.4.3] - 2019-12-11
### Added
- .Net Core 3.0 support

## [Eyes.Windows 2.5.4] - 2019-12-11
### Added
- .Net Core 3.0 support

## [Eyes.CodedUI 2.4.2] - 2019-12-11
### Updated
- Match to latest Eyes.Sdk

## [Eyes.Selenium 2.22.29] - 2019-11-18
### Fixed
- Internet Explorer fixes:
	- Element locations.
	- Capture scripts.
	- Test on Linux based systems.

## [Eyes.Windows 2.5.3] - 2019-11-17
### Fixed
- Reverted minimum .NET Framework version to 4.5.

## [Eyes.CodedUI 2.4.1.3] - 2019-11-17
### Fixed
- Reverted minimum .NET Framework version to 4.5.

## [Eyes.LeanFT 2.6.1.3] - 2019-11-17
### Fixed
- Reverted minimum .NET Framework version to 4.5.

## [Eyes.Sdk 2.22.28.2] - 2019-11-17
### Fixed
- Reverted minimum .NET Framework version to 4.5.

## [Eyes.Selenium 2.22.28.2] - 2019-11-17
### Fixed
- Reverted minimum .NET Framework version to 4.5.

## [Eyes.Appium 4.0.2] - 2019-11-17
### Fixed
- Reverted minimum .NET Framework version to 4.5.
- 
## [Eyes.Images 2.4.2.3] - 2019-11-17
### Fixed
- Reverted minimum .NET Framework version to 4.5.

## [Eyes.Windows 2.5.2] - 2019-11-13
### Added
- Added API for capturing window by handle.
### Fixed
- Fixed SSH communication issue by upgrading .NET Framework version.
### Updated
- .NET Framework version updated to 4.6.1.
- The window to grab doesn't have to be top-most or active anymore.
- Updated dependencies versions.

## [Eyes.Sdk 2.22.28.1] - 2019-11-11
### Fixed
- Added missing member `accessibilityRegions_` to the cloning process.
- [Internal] Tests.

## [Eyes.CodedUI 2.4.1.1] - 2019-11-07
### Updated
- Repacked NuGet together with updated dependencies.

## [Eyes.Images 2.4.1.1] - 2019-11-07
### Updated
- Repacked NuGet together with updated dependencies.

## [Eyes.Windows 2.5.1.1] - 2019-11-07
### Updated
- Repacked NuGet together with updated dependencies.

## [Eyes.LeanFT 2.6.1.1] - 2019-11-07
### Updated
- Repacked NuGet together with updated dependencies.

## [Eyes.Appium 4.0.0] - 2019-11-07
### Updated
- Appium.WebDriver dependency updated to 4.0.0
- Added support for .Net Core and .Net Standard.

## [Eyes.Selenium 2.22.28] - 2019-11-06
### Added
- This CHANGELOG file.
### Updated
- DOM Capture scripts updated to 7.0.18
- DOM Snapshogt scripts updated to 3.0.6
- Updated Eyes.Selenium dependencies:
  * Selenium.WebDriver 3.141.0
  * System.Runtime 4.3.1
- Only report tests to production table if TRAVIS_TAG exists and contains "RELEASE_CANDIDATE".
### Fixed
- Read missing APPLITOOLS_SERVER_URL from environment variable.
- SessionResults metadata objects better match server response (WIP).
- Worked around Chrome 78 CSS translate transform bug.
- [Internal] TestSetup set up correctly SauceLabs and BrowserStack services.
