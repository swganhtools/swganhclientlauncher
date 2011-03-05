using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientLauncher.Usercontrols
{
    /// <summary>
    /// Interaction logic for PatchProgress.xaml
    /// </summary>
    public partial class PatchProgress : UserControl
    {
        private Patcher myPatcher;
        private ServerInfo theServerInfo;
        private UserPreferences myUserPreferences;
        private TextVariables myTextVariables
        {
            get
            {
                return this.DataContext as TextVariables;
            }
        }
        public PatchProgress(ServerInfo siServerInfo, UserPreferences theUserPrefernces)
        {
            InitializeComponent();
            theServerInfo = siServerInfo;
            myUserPreferences = theUserPrefernces;
        }

        public event EventHandler<ErrorMessageEventArgs> OnError;
        public event EventHandler<Patcher.PatchFunctionCompleteEventArgs> PatchComplete;

        public void StartPatch()
        {
            //make the patcher
            myPatcher = new Patcher(myTextVariables, myUserPreferences);
            myPatcher.OnError += new EventHandler<ErrorMessageEventArgs>(myPatcher_OnError);
            myPatcher.PatchStepFired += new EventHandler<Patcher.PatchingEventArgs>(myPatcher_PatchStepFired);
            myPatcher.PatchFunctionComplete += new EventHandler<Patcher.PatchFunctionCompleteEventArgs>(myPatcher_PatchFunctionComplete);

            //and start checking those files
            myPatcher.PatchClient(theServerInfo);
        }

        void myPatcher_PatchFunctionComplete(object sender, Patcher.PatchFunctionCompleteEventArgs e)
        {
            if (PatchComplete != null)
            {
                PatchComplete(this, e);
            }

            myPatcher.Dispose();
        }

        void myPatcher_PatchStepFired(object sender, Patcher.PatchingEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                if (e.NewEvent)
                {
                    //add a new line to split them up
                    lblPatchProgress.Text += Environment.NewLine;
                }
                else
                {
                    lblPatchProgress.Text += "....";
                }
                lblPatchProgress.Text += e.Message;

                svProgress.ScrollToBottom();

            }), System.Windows.Threading.DispatcherPriority.Normal);        
        }

        void myPatcher_OnError(object sender, ErrorMessageEventArgs e)
        {
            if (OnError != null)
            {
                OnError(this, e);
            }
        }

    }
}
