﻿using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += unhandledExceptionHandler;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static void unhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            if (e is ConfigurationErrorsException ex)
            {
                if (ex.InnerException is ConfigurationErrorsException)
                {
                    ex = (ConfigurationErrorsException)ex.InnerException;
                    switch (MessageBox.Show("Error while accessing the configuration file.\n\n" +
                            $"Message:\n{e.Message}\n" +
                            $"{ex.Message}\n" +
                            $"File: {ex.Filename}\n\n" +
                            "Ark Smart Breeding will stop now.\n" +
                            "Should the file be deleted? This might fix it.\n" +
                            "The library file remains untouched.",
                            "Error reading configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Error))
                    {
                        case DialogResult.Yes:
                            File.Delete(ex.Filename);
                            //Properties.Settings.Default.Reload();
                            break;
                    }
                }
                else
                {
                    string folder = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            System.Reflection.Assembly.GetExecutingAssembly().EntryPoint.ReflectedType.Namespace);
                    MessageBox.Show("Error while accessing the configuration file.\n\n" +
                            $"Message:\n{e.Message}\n\n" +
                            "Ark Smart Breeding will stop now.\n" +
                            $"You can try to delete/rename the folder {folder}",
                            "Error reading configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Environment.Exit(0);
            }
            else
            {
                if (System.Diagnostics.Debugger.IsAttached) throw e;
                string message = e.Message
                    + "\n\nMethod throwing the error: " + e.TargetSite.DeclaringType.FullName + "." + e.TargetSite.Name
                    + (e.InnerException != null ? "\n\nInner Exception:\n" + e.InnerException.Message : string.Empty);
                MessageBox.Show("Unhandled Exception:\n\n" + message, "Error in " + e.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
