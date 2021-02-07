using System;
using System.Linq;
using System.Diagnostics;
using DiscordRPC;
using GraphicsGaleRichPresence.Utils;
using System.Threading;
using System.ComponentModel;
using System.Text;

namespace GraphicsGaleRichPresence
{
    class Program
    {
        // have a nice day bruv
        static void Main(string[] args)
        {
            // beforehand stuff
            DiscordRpcClient DiscordClient = new DiscordRpcClient("808005695135285252");
            DiscordClient.Initialize();
            void SetPresence(RichPresence Presence)
            {
                DiscordClient.SetPresence(Presence);
            }
            // MEMREAD
            memread mreadr = new memread();
            //bytesout for BitConverter commands, may not be needed but I added it cuz I wanted to
            int bytesout = 0;
            // strings for rich pres.
            string project = "";
            string state = "";
            // INFINITY!!!
            for (; ; )
            {
                Thread.Sleep(500);
                var timesta = Timestamps.Now;
                for (; ; )
                {
                    // 500ms so CPU usage isn't skyrocketing
                    Thread.Sleep(500);

                    // process stuff lelelel
                    Process process = Process.GetProcessesByName("gale").LastOrDefault();
                    string graphicsGaleWindowName = "";
                    if (process != null)
                    {
                        graphicsGaleWindowName = process.MainWindowTitle;

                        // early memory JUNK
                        IntPtr BaseAddress = process.Modules[0].BaseAddress;
                        mreadr.ReadProcess = process;
                        mreadr.OpenProcess();

                        // GET STRING!!!!!!!
                        uint FilenameAddress = BitConverter.ToUInt32(mreadr.ReadMemory((IntPtr)(BaseAddress + 0x001ED7DC), 4, out bytesout), 0);
                        uint secondary = BitConverter.ToUInt32(mreadr.ReadMemory((IntPtr)(FilenameAddress) + 0x6C, 4, out bytesout), 0);
                        uint third = BitConverter.ToUInt32(mreadr.ReadMemory((IntPtr)secondary + 0x368, 100, out bytesout), 0);
                        var last = mreadr.ReadMemory((IntPtr)third + 0x0, 100, out bytesout);
                        // ENCODING!!!
                        Encoding encoding = Encoding.UTF8;
                        //grabs filename and saves as string
                        string Filename = encoding.GetString(last, 0, 100);
                        try
                        {
                            Filename = Filename.Substring(0, Filename.IndexOf('\x00'));
                        }
                        //if unable to, it will leave Filename the same (very broken string LOL)
                        catch (System.ArgumentOutOfRangeException e)
                        {
                        }
                        // "" for some reason is used on the new image creation screen so ¯\_(ツ)_/¯
                        if (Filename == "")
                        {
                            project = "Creating an untitled project";
                        }
                        // this can be used... ANYWHERE. not sure why
                        else if (Filename == null)
                        {
                            project = "Working on or creating an untitled project";
                        }
                        // this is here because of the stinky startup project string
                        else if (Filename == "�`")
                        {
                            project = "Working on an untitled project";
                        }
                        // normal filename :)
                        else
                        {
                            project = "Working on " + Filename;
                        }
                    }
                    else if (process == null)
                    {
                        graphicsGaleWindowName = "notrunning";
                    }
                    else
                    {
                        graphicsGaleWindowName = "";
                    }
                   
                    if (graphicsGaleWindowName == "notrunning")
                    {
                        project = "GraphicsGale is not open...";
                        state = "Waiting...";
                    }

                    // not much to really explain here. 
                    SetPresence(new RichPresence()
                    {
                        Details = project,
                        State = state,
                        Timestamps = timesta,
                        Assets = new Assets()
                        {
                            LargeImageKey = "gale",
                            LargeImageText = "GraphicsGale",
                            SmallImageText = "",
                            SmallImageKey = ""
                        }
                    });
                }
            }
        }
    }
}
