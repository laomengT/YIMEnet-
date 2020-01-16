﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RimeApi;

namespace YIME.PlayGroundConsole
{


    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            RimeTraits traits = new RimeTraits();
            traits.app_name = "YCIME";
            
            //Rime.RimeSetup(ref traits);
            Console.WriteLine("初始化1");
            Rime.RimeInitialize(IntPtr.Zero);
            Console.WriteLine("初始化2");
            //Rime.RimeSetNotificationHandler(Handler, IntPtr.Zero);
            if (Rime.RimeStartMaintenance(true))
            {
               Rime.RimeJoinMaintenanceThread();
            }
            var rimesessionid = Rime.RimeCreateSession();
            Console.WriteLine("Session ID: " + rimesessionid);
            
           
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape) break;
                Console.WriteLine();
                Rime.RimeSimulateKeySequence(rimesessionid, key.KeyChar.ToString());

                var commit = new RimeCommit();
                var status = new RimeStatus();
                var context = new RimeContext();
                commit.Init();
                status.Init();
                context.Init();
           
                Rime.RimeGetCommit(rimesessionid, ref commit);
                Rime.RimeGetStatus(rimesessionid, ref status);
                status.is_simplified = true;
                Rime.RimeFreeStatus(ref status);
                Rime.RimeGetContext(rimesessionid, ref context);
                Console.WriteLine("Commit:" + commit.text);
                Console.WriteLine($"Status: {status.is_composing},{status.is_ascii_mode}, {status.is_simplified},{status.is_traditional},{status.schema_name}");
                Console.WriteLine($"Context: {context.commit_text_preview}, {context.composition.preedit}");
                var candiates =
                    RimeApi.Common.StuctArrayFromIntPtr<RimeCandidate>(context.menu.candidates,
                        context.menu.num_candidates);
                for (int i = 0; i < context.menu.num_candidates; i++)
                {
                    Console.WriteLine($"{i+1}: {candiates[i].text}");
                }



            }

        }

        private static void Handler(IntPtr contextObject, UIntPtr sessionId, IntPtr messageType, IntPtr messageValue)
        {
            string t = Common.StringFromNativeUtf8(messageType);
            string v = Common.StringFromNativeUtf8(messageValue);
            Console.WriteLine($"Call Handler: {t}: {v}");
        }
    }
}
