using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using BSTVSIX.百师通_翻译_在线版;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace BSTVSIX.Command
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class 百师通_翻译_在线版_
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("8e3854ca-f2fc-4366-b673-3678d90302cf");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="百师通_翻译_在线版_"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private 百师通_翻译_在线版_(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static 百师通_翻译_在线版_ Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in 百师通_翻译_在线版_'s constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new 百师通_翻译_在线版_(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            string title = "百师通-翻译(在线版)";

            //-------------获取选中内容
            DTE dte = ServiceProvider.GetServiceAsync(typeof(DTE)).Result as DTE;
            //var file = dte.FileName;
            //var doc = dte.ActiveDocument;
            //var item = doc.ProjectItem;
            //var filename = item.FileNames[0];
            string selectTXT = "";
            if (dte.ActiveDocument != null && dte.ActiveDocument.Type == "Text")
            {
                var selection = (TextSelection)dte.ActiveDocument.Selection;
                string text = selection.Text;
                selectTXT = text; //直接用selection.Text无效
            }
            if (string.IsNullOrWhiteSpace(selectTXT))
            {
                ShowTxt(waringTxt[new Random().Next(0, 3)], title, OLEMSGICON.OLEMSGICON_WARNING);
            }
            else
            {
                string afterTXT = FanYi(selectTXT);
                ShowTxt(afterTXT, title, OLEMSGICON.OLEMSGICON_QUERY);
            }
        }

        private string FanYi(string selectTXT)
        {
            TransApi transApi = new TransApi();
            return transApi.FanYi(selectTXT);
        }

        public string[] waringTxt = new string[] { "你在开玩笑吧！", "键盘给你，你来翻译！",
            "没有内容，翻译不了！" };
        private void ShowTxt(string content, string title, OLEMSGICON oLEMSGICON = OLEMSGICON.OLEMSGICON_NOICON)
        {
            VsShellUtilities.ShowMessageBox(
                    this.package,
                    content,
                    title,
                    oLEMSGICON,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
