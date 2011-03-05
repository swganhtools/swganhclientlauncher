# [SWG:ANH][1] Client Launcher #

This client launcher and it's associated projects are a tool to help launch the [SWG client][2] by 'pointing' it to the different galaxies that will eventually be availble via the [various communites][3] looking to host their own servers.  The launcher dials home to maintain a list of servers which the end-user can connect to.

## Projects and their usage ##

The launcher comprises of three Visual Studio Solutions:

### Client Launcher ###
This is a .net 4 application written in WPF.  It's skinnable, to a certain degree, to allow communites to roll their own version suited to their individual tastes.  There is a sub-project which wrpas the client up into a handy Windows installers for easy deployment
*This project opens in Visual Studio 2010

### PacketHandler ###
A tiny c++ class to handle the packet encryption/decryption required to initially talk to the server to open a session of the [SWG Client][2] to connect to.  It uses the standard zLib 1.2.5 library for compression/decompression
*This project opens in Visual Studio 2008

### LauncherData ###
A WCF service which the launcher connects to.  This serves information on the various servers and will be a central point to hold server information going-forward.  The servers will be able to update their information with things like population and server news
*This project opens in Visual Studio 2010

We hope you find this tool useful but please feel free to talk to us in IRC if you need access of have some features you'd like to see, or even write yourself




[1]: http://www.swganh.com
[2]: http://www.starwarsgalaxies.com
[3]: http://www.swganh.com/anh_community