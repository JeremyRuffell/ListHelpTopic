using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AquiraHelpTopics
{
    class Program
    {
        // List that holds Class's HelpTopic Values for the class's that have a bug.
        public static List<Item> myList = new List<Item>();

        static void Main(string[] args)
        {
            // Finish Upadate if one was prevously started.
            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            IEnumerable<FileInfo> taskFiles = dir.GetFiles().Where(p => p.Name.StartsWith("old_"));
            foreach (FileInfo item in taskFiles)
            {
                File.SetAttributes(item.ToString(), FileAttributes.Normal);
                File.Delete(item.ToString());
            }

            // Get Latest Version.
            HttpClient httpClient = new HttpClient();

            // GitHubs API requires a User Agent.
            httpClient.DefaultRequestHeaders.Add("User-Agent", "AquiraHelpTopics");
            
            // Get the Repo info from GitHubs API.
            Task<string> APIRequest = httpClient.GetStringAsync("https://api.github.com/repos/JeremyRuffell/AquiraHelpTopics/releases/latest");
            string APIResponse = APIRequest.Result;

            // Deserialize the Response from the GitHub API.
            GithubAPIResponse deserializedObject = JsonConvert.DeserializeObject<GithubAPIResponse>(APIResponse);

            // Try Parse the Latest Version from the Latest Release tag name given from the GitHub API.
            Version LatestVersion = null;
            Version.TryParse(deserializedObject.tag_name, out LatestVersion);

            // Try Parse the Latest Version from the Latest Release tag name given from the GitHub API.
            Version ApplicationVersion = null;
            Version.TryParse(Assembly.GetExecutingAssembly().GetName().Version.ToString(), out ApplicationVersion);

            if (ApplicationVersion < LatestVersion)
            {
                Console.Write("Updating from ");
                Chalk(ApplicationVersion.ToString(), ConsoleColor.DarkYellow);
                Console.Write(" > ");
                Chalk(LatestVersion.ToString(), ConsoleColor.Green);
                Console.WriteLine();

                // Rename all files.
                
                DirectoryInfo directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory); // (exception here)
                Console.WriteLine("App directory is: "+ directory.FullName);
                FileInfo[] files = directory.GetFiles();

                foreach (FileInfo file in files)
                {
                    File.Move(file.FullName, Path.Combine(directory.FullName, $"old_{file.Name}"));
                }

                Common.DownloadLatestRelease();
                Common.InstallLatestRelease();

                Console.Clear();
                Console.WriteLine("Application Updated, Please relaunch the applicaiton.\nPress any key to continue . . .");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else if (ApplicationVersion == LatestVersion)
            {
                Console.Write("Running latest version [");
                Chalk(ApplicationVersion.ToString(), ConsoleColor.Green);
                Console.WriteLine("]\n");
            }

            AddExceptionClasses();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Load DLL. 
            // Note: Aquira_WinControls is the only DLL that contains the wanted Interfaces.
            Assembly assembly = Assembly.LoadFrom(Path.Combine(Common.GetAquiraDirectory(), "Aquira_WinControls.dll"));

            //Get Types from DLL.
            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                Type Interface = type.GetInterfaces().Where(x => x.Name == "IGetHelpTopic").FirstOrDefault();
                if (Interface != null)
                {
                    string check = Common.CheckIfValid(type.Name);
                    if (check != "Ok")
                    {
                        Common.SpaceGenerator(type.Name, check, "Warning");
                    }
                    else
                    {
                        ConstructorInfo ci = type.GetConstructor(new Type[] { });

                        object v = ci.Invoke(null);

                        MethodInfo m = type.GetProperty("HelpTopic").GetMethod;

                        object o = m.Invoke(v, null);

                        Common.SpaceGenerator(type.Name, o.ToString(), "Success");
                    }
                }
            }

            // Debugging Purposes.
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey();
        }

       public static void Chalk(string s, ConsoleColor c)
        {
            Console.ForegroundColor = c;
            Console.Write(s);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void AddExceptionClasses()
        {
            myList.Add(new Item { ClassName = "WaveImportForm", HelpTopicReturnValue = "Setup_Wave" });
            myList.Add(new Item { ClassName = "PaymentNavigation", HelpTopicReturnValue = "Accounting_Payments" });
            myList.Add(new Item { ClassName = "InvoiceExport", HelpTopicReturnValue = "Accounting_InvoiceExport" });
            myList.Add(new Item { ClassName = "InvoicePreviewNavigation", HelpTopicReturnValue = "Accounting_Invoices" });
            myList.Add(new Item { ClassName = "GenerateEstimatedGRPForm", HelpTopicReturnValue = "Setup_Stations_Stations_GenerateEstimatedGRPDateRangesForm" });
            myList.Add(new Item { ClassName = "EBillingExport", HelpTopicReturnValue = "Accounting_EBillingExport" });
            myList.Add(new Item { ClassName = "MediaPurgeConfirmationForm", HelpTopicReturnValue = "Creative_Media_PurgeMedia" });
            myList.Add(new Item { ClassName = "MediaPurgeForm", HelpTopicReturnValue = "Creative_Media_PurgeMedia" });
            myList.Add(new Item { ClassName = "MediaHistoryNavigation", HelpTopicReturnValue = "Creative_Media_Detail_History" });
            myList.Add(new Item { ClassName = "MediaNavigation", HelpTopicReturnValue = "Creative_Media" });
            myList.Add(new Item { ClassName = "ProposedContractSummariesNavigation", HelpTopicReturnValue = "Traffic_Contracts" });
            myList.Add(new Item { ClassName = "ReportNavigation", HelpTopicReturnValue = "Reports_Reports" });
            myList.Add(new Item { ClassName = "ReportDetailInformation", HelpTopicReturnValue = "Reports_Reports_General" });
            myList.Add(new Item { ClassName = "ReportEditor", HelpTopicReturnValue = "Reports_Reports_Editor" });
            myList.Add(new Item { ClassName = "FillerContainer", HelpTopicReturnValue = "Traffic_Log_Fillers" });
            myList.Add(new Item { ClassName = "MakeGoodSpotsForm", HelpTopicReturnValue = "Traffic_Logs_MakeGoodSpots" });
            myList.Add(new Item { ClassName = "ExportData", HelpTopicReturnValue = "Accounting_ExportInvoicesData_ManualExport" });
            myList.Add(new Item { ClassName = "BillingPlan", HelpTopicReturnValue = "Traffic_Contracts_BillingPlan" });
            myList.Add(new Item { ClassName = "ContractOptions", HelpTopicReturnValue = "Traffic_ContractDetails_ContractOptions" });
            myList.Add(new Item { ClassName = "BrowseContractsForm", HelpTopicReturnValue = "Traffic_Contracts_Spotline_CopyFrom" });
            myList.Add(new Item { ClassName = "EndContractForm", HelpTopicReturnValue = "Traffic_Contracts_EndContract" });
            myList.Add(new Item { ClassName = "ProposedContractMediaLinesNavigation", HelpTopicReturnValue = "NULL" });
            myList.Add(new Item { ClassName = "SpotlineSplitWizard", HelpTopicReturnValue = "Traffic_Contracts_SpotLines_SpotLineSplitWizard" });
            myList.Add(new Item { ClassName = "MediaLinesNavigation", HelpTopicReturnValue = "Traffic_Contracts_Media" });
            myList.Add(new Item { ClassName = "ContractSummariesNavigation", HelpTopicReturnValue = "Traffic_Contracts" });
            myList.Add(new Item { ClassName = "PaymentAnalysisNavigation", HelpTopicReturnValue = "(_overdueFlag) Reports_OverdueAnalysis or Reports_CommissionableAnalysis" });
            myList.Add(new Item { ClassName = "LogFormatAssignmentCalendarNavigation", HelpTopicReturnValue = "Setup_Clocks_LogFormatAssignments_Calendar" });
            myList.Add(new Item { ClassName = "SelectSalesRepsForm", HelpTopicReturnValue = "Traffic_Clients_AddOrRemoveSalesReps" });
            myList.Add(new Item { ClassName = "BalanceCurrentMonthNavigation", HelpTopicReturnValue = "Accounting_BalanceCurrentMonth" });
            myList.Add(new Item { ClassName = "ApprovalLevelNavigation", HelpTopicReturnValue = "Setup_AprovalLevels" });
            myList.Add(new Item { ClassName = "ReportsEmailSettings", HelpTopicReturnValue = "Setup_GlobalSettings_GlobalSettings_Report" });
            myList.Add(new Item { ClassName = "Contract", HelpTopicReturnValue = "Setup_GlobalSettings_GlobalSettings_Contract" });
            myList.Add(new Item { ClassName = "ReportFilterSearchControl", HelpTopicReturnValue = "NULL" });
            myList.Add(new Item { ClassName = "RCSAdvancedSearchControl", HelpTopicReturnValue = "NULL" });
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                // The app is closing anyway, supress the error and hard-exit now!
                // This avoids the subsequent unhandled exception we were getting
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("ERROR - Unhandled App Exception! " + e.ExceptionObject);
            }
        }
    }
}
