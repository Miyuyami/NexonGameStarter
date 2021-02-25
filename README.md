# Requirements
* .NET Desktop Runtime 5: https://dotnet.microsoft.com/download/dotnet/5.0
* WebView2 Runtime (recommended to use Evergreen Bootstrapper), the link is also prompted on startup if the browser initialization fails: https://developer.microsoft.com/en-us/microsoft-edge/webview2/#download-section

# How to use
* you must have the NGM (Nexon Game Manager) installed.
* you must find out the GameId of the game you want to start.
* you must pass it as the first argument to the app, then you just have to run the application and login, it opens the browser where you type your login information.

When you want to directly launch the game you choose the Launch option and when you want/need to patch, just choose the Patch option. 
Some games do not allow different modes (Launch/Patch).

By default this uses the Korean locale and login url, to change those is again by arguments.
Second argument is the locale and the third argument is the login url.

# Games tested and GameIds
* Elsword: 94224
* MapleStory 2: 106498
