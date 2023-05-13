# Lyngdorf Browser

<p align="center">
  <a href="https://github.com/michaelmsonne/LyngdorfBrowser"><img src="https://img.shields.io/github/languages/top/michaelmsonne/LyngdorfBrowser.svg"></a>
  <a href="https://github.com/michaelmsonne/LyngdorfBrowser"><img src="https://img.shields.io/github/languages/code-size/michaelmsonne/LyngdorfBrowser.svg"></a>
  <a href="https://github.com/michaelmsonne/LyngdorfBrowser"><img src="https://img.shields.io/github/downloads/michaelmsonne/LyngdorfBrowser/total.svg"></a>
</p>

## Download

[Download the latest version](../../releases/latest)

[Version History](CHANGELOG.md)

# Introduction 
Welcome to the Lyngdorf Amplifier Finder!

This project provides a small browser that enables you to find Lyngdorf amplifiers on your local network. The browser is built using C# WinForms and uses networking and web technologies to scan your network and identify any Lyngdorf Appfhiller web applications that may be running on your network.

To get started, simply start the application and the browser will scan your local network for any Lyngdorf Appfhiller web applications and display them in the browser window. If a Lyngdorf amplifier is found, it will show up in the application!

Happy browsing!

# How the code is working
This code attempts to find the IP address of a Lyngdorf device on the local network using its MAC address. The MAC address is a unique identifier assigned to network interfaces, and it can be used to identify devices on the network.

The code first calls the FindIpFromMacAddress method of the IpMacMapper class, passing the MAC address of the Lyngdorf device as a parameter. This method returns the IP address associated with the specified MAC address, if any.

If an IP address is found, the code creates a new instance of the ChromiumWebBrowser class, passing the IP address as the URL to display. This creates a new browser window in the application that displays the web interface of the Lyngdorf device.

If no IP address is found, the code displays a message box indicating that no Lyngdorf devices were found on the network. If an error occurs during the IP address lookup, the code displays a message box indicating that the operation failed and the device may not be supported.

The code also sets up an event handler for the TitleChanged event of the ChromiumWebBrowser control. This event is triggered when the title of the browser window changes, which can happen when the user navigates to a different page or performs an action in the Lyngdorf web interface. The event handler updates the title of the main application window to reflect the title of the browser window, providing a more seamless browsing experience for the user.