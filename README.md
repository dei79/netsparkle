# netsparkle
NetSparkle is an easy-to-use software update framework for .NET developers on Windows, MAC or Linux. It was 
inspired by the Sparkle (http://bit.ly/HWyJd) project for Cocoa developers and the WinSparkle (http://bit.ly/cj5kP5) 
project (a Win32 port).

![NetSparkle](https://github.com/dei79/netsparkle/blob/master/Assets/NetSparkle.png)

This framework contains a lot of features, please check them out:
* True self-updating, no work required from user based on Windows Installer packages
* Uses appcasts for release information
* Displays release news to the user via Internet Explorer Control
* Displays a detailed progress window to the user
* NetSparkle requires no code in your app, so it's trivial to upgrade or remove the module
* Seamless integration—there's no mention of NetSparkle; your icons and app name are used
* Supports DSA signatures for ultra-secure updates
* UpdateChecker-Helper for start menu integration
* i18n support see [localization section](https://github.com/dei79/netsparkle/wiki/Localization)

In the future we are planning to realize all the other well know Sparkle features as well:
* Optionally sends user demographic information to the server when checking for updates
* NetSparkle can install .exe files for wider installer support
* Support for custom consumer logic when a update was detected
