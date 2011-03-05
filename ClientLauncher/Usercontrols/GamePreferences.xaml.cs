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
using Microsoft.Win32;
using System.Diagnostics;
namespace ClientLauncher.Usercontrols
{
    /// <summary>
    /// Game Preferences handles all of the ini file settings for the SWG Client
    /// This page also handles some settings for the launcher itself
    /// </summary>
    public partial class GamePreferences : UserControl
    { 
        #region Initialisation

        private TextVariables myVariables
        {
            get
            {
                return this.DataContext as TextVariables;
            }
        }

        public enum ValuesToLoad
        {
            Graphics,
            Sound,
            Game,
            Advanced
        }

        public enum ChangedSetting
        {
            Skin,
            Pallete, 
            GhostFont
        }

        public class SettingsChangedEventArgs : EventArgs
        {
            public ChangedSetting WhatChanged
            {
                get;
                private set;
            }

            public string SettingName
            {
                get;
                private set;
            }

            public SettingsChangedEventArgs(ChangedSetting theChangedSetting, string strTheSettingName)
            {
                this.WhatChanged = theChangedSetting;
                this.SettingName = strTheSettingName;
            }

        }

        public event EventHandler<SettingsChangedEventArgs> SettingsHaveChanged; 
        private bool isInitialising = false;
        private UISettings theSettings;
        private UserPreferences myPrefs;
        public GamePreferences(UISettings parentSettings, UserPreferences parentPrefs)
        {
            InitializeComponent();       

            ScreenResolution myScreens = new ScreenResolution();
            List<ScreenResolution.AvailableScreen> lstRes = myScreens.GetScreenResolutions();

            theSettings = parentSettings;
            myPrefs = parentPrefs;
            this.Loaded += new RoutedEventHandler(GamePreferences_Loaded);
        }

        void GamePreferences_Loaded(object sender, RoutedEventArgs e)
        {
            tcGameOptions.SelectionChanged += new SelectionChangedEventHandler(tcGameOptions_SelectionChanged);
            LoadValues(ValuesToLoad.Graphics);
                        
            //and always load the skins, pallettes and ghost fonts
            cboPallette.ItemsSource = theSettings.GetAvailablePallettes;
            cboSkin.ItemsSource = theSettings.GetAvailableSkins;
            cboGhostFont.ItemsSource = theSettings.GetFonts;

            isInitialising = true;

            cboPallette.SelectedItem = myPrefs.CurrentPallette;
            cboSkin.SelectedItem = myPrefs.CurrentSkin;
            cboGhostFont.SelectedItem = myPrefs.CurrentGhostFont;
            isInitialising = false;
        }

        void tcGameOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl iWasChanged = sender as TabControl;
            TabItem iWasSelected = iWasChanged.SelectedItem as TabItem;

            switch (iWasSelected.Name.ToLowerInvariant())
            {
                case "tbigraphics":
                    LoadValues(ValuesToLoad.Graphics);
                    break;
                case "tbisound":
                    LoadValues(ValuesToLoad.Sound);
                    break;
                case "tbigame":
                    LoadValues(ValuesToLoad.Game);
                    break;
                case "tbiadvanced":
                    LoadValues(ValuesToLoad.Advanced);
                    break;
            }
        }        

        private void LoadValues(ValuesToLoad theValuesToLoad)
        {
            isInitialising = true;
            //open the ini files and check the current values
            IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");

            switch (theValuesToLoad)
            {
                case ValuesToLoad.Graphics:
                    #region Graphics Options

                    chkWindowedMode.IsChecked = myFiles.GetInt32("ClientGraphics", "windowed", 0) == 1;
                    chkSafeRenderer.IsChecked = myFiles.GetInt32("ClientGraphics", "rasterMajor", 7) == 5 && myFiles.GetInt32("ClientGraphics", "useSafeRenderer", 0) == 1;
                    chkBorderlessWindow.IsChecked = myFiles.GetInt32("ClientGraphics", "borderlessWindow", 0) == 1;
                    chkDisableFastMouseCursor.IsChecked = myFiles.GetInt32("ClientUserInterface", "alwaysSetMouseCursor", 0) == 1;
                    chkDisableHardwareMouseCursor.IsChecked = myFiles.GetInt32("ClientGraphics", "useHardwareMouseCursor", 0) == 0;
                    chkDisableVSynch.IsChecked = myFiles.GetInt32("Direct3d9", "allowTearing", 0) == 1;
                    chkUseLowDetailNormalMaps.IsChecked = myFiles.GetInt32("ClientGraphics", "discardHighestNormalMipMapLevels", 0) == 1;
                    chkUseLowDetailTextures.IsChecked = myFiles.GetInt32("ClientGraphics", "discardHighestMipMapLevels", 0) == 1;
                    chkConstrainMouseCursor.IsChecked = myFiles.GetInt32("ClientGraphics", "constrainMouseCursorToWindow", 1) == 1;
                    chkShowDebugWindow.IsChecked = myFiles.GetInt32("ClientGame", "debugPrint", 0) == 1;
                    //some keys are special
                    //these use the same key name in the same section
                    List<KeyValuePair<string, string>> lstKeys = myFiles.GetSectionValuesAsList("ClientGraphics");
                    chkDisableMultiPassRendering.IsChecked = lstKeys.Any(kvp => kvp.Key.Equals("disableOptionTag", StringComparison.InvariantCultureIgnoreCase) && kvp.Value.Equals("HIQL"));
                    chkDisableBumpMapping.IsChecked = lstKeys.Any(kvp => kvp.Key.Equals("disableOptionTag", StringComparison.InvariantCultureIgnoreCase) && kvp.Value.Equals("DOT3"));

                    //load the available screen resolutions
                    ScreenResolution myScreenReses = new ScreenResolution();
                    List<ScreenResolution.AvailableScreen> lstScreenRes = myScreenReses.GetScreenResolutions();

                    //what is it currently set to?
                    int nAdapter = myFiles.GetInt32("Direct3d9", "adapter", 0);
                    int nWidth = myFiles.GetInt32("ClientGraphics", "screenWidth", 1024);
                    int nHeight = myFiles.GetInt32("ClientGraphics", "screenHeight", 768);
                    int nFrequency = myFiles.GetInt32("Direct3d9", "fullscreenRefreshRate", 60);
                    ScreenResolution.ScreenRes theCurrentScreenRes = new ScreenResolution.ScreenRes
                    {
                        Frequency = 60,
                        Width = 1024,
                        Height = 768
                    };

                    //people can remvoe monitors
                    if (!lstScreenRes.Any(sr => sr.AdapterNumber == nAdapter))
                    {
                        nAdapter = 0;
                        myFiles.WriteValue("Direct3d9", "adapter", 0);
                    }
                    var currentRes = lstScreenRes.Where(sr => sr.AdapterNumber == nAdapter).First().AvailableResolutions.Where(sr => sr.Width == nWidth && sr.Height == nHeight && sr.Frequency == nFrequency);

                    if (currentRes.Count() > 0)
                    {
                        theCurrentScreenRes = currentRes.First();
                    }

                    var currentAdapter = lstScreenRes.Where(sr => sr.AdapterNumber == nAdapter).First();

                    cboResolution.ItemsSource = currentAdapter.AvailableResolutions;
                    cboResolution.SelectedItem = theCurrentScreenRes;

                    cboAdapter.ItemsSource = lstScreenRes;
                    cboAdapter.SelectedItem = currentAdapter;

                    if (lstScreenRes.Count == 1)
                    {
                        //can't change it anyway
                        lblAdapter.IsEnabled = false;
                        cboAdapter.IsEnabled = false;
                    }

                    //and add the shader versions
                    cboShaderVersion.ItemsSource = ShaderVersions;
                    cboShaderVersion.SelectedItem = CurrentShaderVersion;

                    #endregion
                    break;
                case ValuesToLoad.Sound:
                    #region Sound Options
                                       
                    //get the miles version
                    lblMilesVersion.Text = FileVersionInfo.GetVersionInfo(SWGANHPAth + "Mss32.dll").FileVersion;

                    this.Dispatcher.BeginInvoke(new Action(delegate
                        {   //Audio Enabled                    
                            chkDisableAudio.IsChecked = myFiles.GetInt32("ClientAudio", "disableMiles", 0) == 1;
                        }), System.Windows.Threading.DispatcherPriority.ContextIdle);
                    cboSpeakers.ItemsSource = SoundVersions;
                    cboSpeakers.SelectedItem = myFiles.GetString("ClientAudio", "soundProvider", "Windows Speaker Configuration");

                    #endregion
                    break;
                case ValuesToLoad.Game:
                    #region Game Options
                    cboLanguage.ItemsSource = Languages;
                    cboLanguage.SelectedItem = CurrentLanguage;

                    this.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            chkSkipIntroSequence.IsChecked = myFiles.GetInt32("ClientGame", "skipIntro", 0) == 1;
                        }),  System.Windows.Threading.DispatcherPriority.ContextIdle);

                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        chkDisableCharSLOD.IsChecked = myFiles.GetInt32("ClientSkeletalAnimation", "lodManagerEnable", 1) == 0;                        
                    }), System.Windows.Threading.DispatcherPriority.ContextIdle);

                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        chkAllowMultipleInstances.IsChecked = myFiles.GetInt32("SwgClient", "allowMultipleInstances", 0) == 1;
                    }), System.Windows.Threading.DispatcherPriority.ContextIdle);
                    #endregion
                    break;
                case ValuesToLoad.Advanced:
                    #region Advanced Options
                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        chkDisableWorldPreloading.IsChecked = myFiles.GetInt32("ClientGame", "preloadWorldSnapshot", 1) == 0;
                    }), System.Windows.Threading.DispatcherPriority.ContextIdle);
                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        chkUseLowDetailCharacters.IsChecked = myFiles.GetInt32("ClientSkeletalAnimation", "skipL0", 0) == 1;
                    }), System.Windows.Threading.DispatcherPriority.ContextIdle);
                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        chkUseLowDetailMeshes.IsChecked = myFiles.GetInt32("ClientObject/DetailAppearanceTemplate", "skipL0", 0) == 1;
                    }), System.Windows.Threading.DispatcherPriority.ContextIdle);
                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        chkDisableTextureBaking.IsChecked = myFiles.GetInt32("ClientTextureRenderer", "disableTextureBaking", 0) == 1;
                    }), System.Windows.Threading.DispatcherPriority.ContextIdle);
                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        chkDisableFileCaching.IsChecked = myFiles.GetInt32("SharedUtility", "disableFileCaching", 0) == 1;
                    }), System.Windows.Threading.DispatcherPriority.ContextIdle);
                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        chkDisableAsynchronousLoader.IsChecked = myFiles.GetInt32("SharedFile", "enableAsynchronousLoader", 1) == 0;
                    }), System.Windows.Threading.DispatcherPriority.ContextIdle);
                    #endregion
                    break;
            }

            isInitialising = false;
        }

        #endregion

        #region Ini File Path

        private string strSWGANHPath;

        public event EventHandler<ErrorMessageEventArgs> OnError;

        public void MessageBoxOK(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(strSWGANHPath))
            {
                System.Windows.Forms.FolderBrowserDialog dia = new System.Windows.Forms.FolderBrowserDialog();

                if (dia.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    strSWGANHPath = dia.SelectedPath;
                }

                //add a slash if it needs one
                if (!strSWGANHPath.EndsWith("\\"))
                {
                    strSWGANHPath += "\\";
                }

                //save this one
                myPrefs.UpdateSettings(UserPreferences.SettingsType.ClientPath, strSWGANHPath);
            }          
        }

        private string SWGANHPAth
        {
            get
            {
                if (String.IsNullOrEmpty(strSWGANHPath))
                {
                    //try and find it in the registry
                    RegistryKey myRegistry = Registry.CurrentUser;
                    RegistryKey regSWGAppPath = myRegistry.OpenSubKey("SOFTWARE\\SWGANH Client");
                    if (regSWGAppPath == null)
                    {
                        //try and get it from their custom path
                        strSWGANHPath = myPrefs.ClientPath;

                        if (string.IsNullOrEmpty(strSWGANHPath))
                        {
                            //or ask the user to specify
                            if (OnError != null)
                            {
                                OnError(this, new ErrorMessageEventArgs(myVariables.RegistryErrorOne + Environment.NewLine + myVariables.RegistryErrorTwo + Environment.NewLine + "(e.g C:\\Program Files\\SWGANH Client", new EventHandler(MessageBoxOK)));
                            }
                        }
                        else
                        {
                            //add a slash if it needs one
                            if (!strSWGANHPath.EndsWith("\\"))
                            {
                                strSWGANHPath += "\\";
                            }
                        }
                    }
                    else
                    {
                        strSWGANHPath = regSWGAppPath.GetValue("").ToString();
                        //add a slash if it needs one
                        if (!strSWGANHPath.EndsWith("\\"))
                        {
                            strSWGANHPath += "\\";
                        }
                    }
                }

                return strSWGANHPath;
            }
        }

        #endregion

        #region Click Handler to make TextBlocks Click thier companion CheckBox
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Get the Container of the clicked element
            TextBlock iWasClicked = sender as TextBlock;
            Canvas iAmTehParent = iWasClicked.Parent as Canvas;

            //Intergorate all the children
            CheckBox chkCheckMe = null;
            foreach (UIElement theElement in iAmTehParent.Children)
            {
                //and find the checkbox within in
                chkCheckMe = theElement as CheckBox;

                if (chkCheckMe != null)
                {
                    chkCheckMe.IsChecked = !chkCheckMe.IsChecked.Value;
                }
            }
        }

        #endregion

        #region CheckBox state handlers to configure the ini file

        #region Graphics

        #region Windowed Mode

        private void chkWindowedMode_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGraphics", "windowed", 1);
            }            
        }

        private void chkWindowedMode_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGraphics", "windowed", 0);
            }            
        }

        #endregion

        #region Borderless Window
        private void chkBorderlessWindow_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGraphics", "borderlessWindow", 1);
            }
        }       

        private void chkBorderlessWindow_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGraphics", "borderlessWindow", 0);
            }
        }

        #endregion

        #region Disable Bump Mapping
        private void chkDisableBumpMapping_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGraphics", "disableOptionTag", "DOT2");

                if (chkDisableMultiPassRendering.IsChecked.Value)
                {
                    myFiles.AddDuplicateKey("ClientGraphics", "disableOptionTag", "HIQL");
                }
            }            
        }

        private void chkDisableBumpMapping_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.DeleteKey("ClientGraphics", "disableOptionTag");

                if (chkDisableMultiPassRendering.IsChecked.Value)
                {
                    myFiles.WriteValue("ClientGraphics", "disableOptionTag", "HIQL");
                }
            }            
        }
        #endregion

        #region Low Detail Textures

        private void chkUseLowDetailTextures_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGraphics", "discardHighestMipMapLevels", 1);
            }            
        }

        private void chkUseLowDetailTextures_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.DeleteKey("ClientGraphics", "discardHighestMipMapLevels");
            }            
        }
        #endregion

        #region Low Detail Normal Maps
        private void chkUseLowDetailNormalMaps_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGraphics", "discardHighestNormalMipMapLevels", 1);
            }            
        }

        private void chkUseLowDetailNormalMaps_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.DeleteKey("ClientGraphics", "discardHighestNormalMipMapLevels");
            }            
        }

        #endregion

        #region V-Synch / Tearing

        private void chkDisableVSynch_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("Direct3d9", "allowTearing", 1);
            }            
        }

        private void chkDisableVSynch_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("Direct3d9", "allowTearing", 0);
            }            
        }

        #endregion

        #region Fast Mouse Cursor
        private void chkDisableFastMouseCursor_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientUserInterface", "alwaysSetMouseCursor", 1);
            }            
        }

        private void chkDisableFastMouseCursor_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.DeleteSection("ClientUserInterface");
            }
        }

        #endregion

        #region Safe Renderer

        private void chkSafeRenderer_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGraphics", "rasterMajor", 5);
                myFiles.WriteValue("ClientGraphics", "useSafeRenderer", 1);
            }            
        }

        private void chkSafeRenderer_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGraphics", "rasterMajor", 7);
                myFiles.WriteValue("ClientGraphics", "useSafeRenderer", 0);
            }           
        }

        #endregion

        #region Disable Multi Pass Rendering

        private void chkDisableMultiPassRendering_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGraphics", "disableOptionTag", "HIQL");

                //this key is duplicated with disable Bump mapping
                if (chkDisableBumpMapping.IsChecked.Value)
                {
                    myFiles.AddDuplicateKey("ClientGraphics", "disableOptionTag", "DOT3");
                }
            }                        
        }        

        private void chkDisableMultiPassRendering_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.DeleteKey("ClientGraphics", "disableOptionTag");

                //this key is duplicated with disable Bump mapping
                if (chkDisableBumpMapping.IsChecked.Value)
                {
                    myFiles.WriteValue("ClientGraphics", "disableOptionTag", "DOT3");
                }
            }            
        }

        #endregion

        #region Disable Hardware Mouse Cursor
        private void chkDisableHardwareMouseCursor_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGraphics", "useHardwareMouseCursor", "0");
            }           
        }

        private void chkDisableHardwareMouseCursor_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGraphics", "useHardwareMouseCursor", "1");
            }            
        }

        #endregion       

        #region Constrain Mouse Cursor to window

        private void chkConstrainMouseCursor_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGraphics", "constrainMouseCursorToWindow", 1);
            }
        }

        private void chkConstrainMouseCursor_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGraphics", "constrainMouseCursorToWindow", 0);
            }
        }

        #endregion

        #region Show Debug Window


        private void chkShowDebugWindow_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGame", "debugPrint", 1);
            }  
        }

        private void chkShowDebugWindow_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGame", "debugPrint", 0);
            }  
        }

        #endregion

        #endregion

        #region Sound

        #region Disable Audio
        private void chkDisableAudio_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientAudio", "disableMiles", 1);
            }            
        }

        private void chkDisableAudio_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.DeleteKey("ClientAudio", "disableMiles");
            }            
        }
        #endregion

        #endregion

        #region Game

        #region Intro Sequence
        private void chkSkipIntroSequence_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGame", "skipIntro", 1);
            }            
        }

        private void chkSkipIntroSequence_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.DeleteKey("ClientGame", "skipIntro");

                if (myFiles.GetSectionValuesAsList("ClientGame").Count == 0)
                {
                    myFiles.DeleteSection("ClientGame");
                }
            }            
        }

        #endregion

        #region Character System Level of Detail Manager
        private void chkDisableCharSLOD_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientSkeletalAnimation", "lodManagerEnable", 0);
            }            
        }

        private void chkDisableCharSLOD_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.DeleteKey("ClientSkeletalAnimation", "lodManagerEnable");

                if (myFiles.GetSectionValuesAsList("ClientSkeletalAnimation").Count == 0)
                {
                    myFiles.DeleteSection("ClientSkeletalAnimation");
                }
            }            
        }

        #endregion

        #region All Multiple Instances

        private void chkAllowMultipleInstances_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("SwgClient", "allowMultipleInstances", 1);
            }

        }

        private void chkAllowMultipleInstances_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.DeleteKey("SwgClient", "allowMultipleInstances");
            }
        }     

        #endregion

        #endregion

        #region Advanced

        #region World Preloading
        private void chkDisableWorldPreloading_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientGame", "preloadWorldSnapshot", 0);
            }            
        }

        private void chkDisableWorldPreloading_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.DeleteKey("ClientGame", "preloadWorldSnapshot");

                if (myFiles.GetSectionValuesAsList("ClientGame").Count == 0)
                {
                    myFiles.DeleteSection("ClientGame");
                }
            }           
        }

        #endregion

        #region Low Detail Characters
        private void chkUseLowDetailCharacters_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientSkeletalAnimation", "skipL0", 1);
            }            
        }

        private void chkUseLowDetailCharacters_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.DeleteKey("ClientSkeletalAnimation", "skipL0");

                if (myFiles.GetSectionValuesAsList("ClientSkeletalAnimation").Count == 0)
                {
                    myFiles.DeleteSection("ClientSkeletalAnimation");
                }
            }
        }

        #endregion

        #region Low Detail Meshes
        private void chkUseLowDetailMeshes_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientObject/DetailAppearanceTemplate", "skipL0", 1);
            }            
        }

        private void chkUseLowDetailMeshes_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.DeleteSection("ClientObject/DetailAppearanceTemplate");
            }            
        }
        #endregion

        #region Texture Baking

        private void chkDisableTextureBaking_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientTextureRenderer", "disableTextureBaking", 1);
            }            
        }

        private void chkDisableTextureBaking_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.DeleteSection("ClientTextureRenderer");
            }            
        }

        #endregion

        #region File Caching
        private void chkDisableFileCaching_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("SharedUtility", "disableFileCaching", 1);
            }            
        }

        private void chkDisableFileCaching_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.DeleteKey("SharedUtility", "disableFileCaching");
            }
        }
        #endregion

        #region Asynchronous Loader

        private void chkDisableAsynchronousLoader_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("SharedFile", "enableAsynchronousLoader", 0);
            }           
        }

        private void chkDisableAsynchronousLoader_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.DeleteSection("SharedFile");
            }            
        }

        #endregion

        #endregion

        #endregion

        #region Drop Downs

        #region Graphics

        #region Display Adapter

        private void cboAdapter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialising)
            {
                ScreenResolution.AvailableScreen theAvailableScreen = cboAdapter.SelectedItem as ScreenResolution.AvailableScreen;

                if (theAvailableScreen != null)
                {
                    IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");

                    if (theAvailableScreen.AdapterNumber == 0)
                    {
                        myFiles.DeleteKey("Direct3d9", "adapter");
                    }
                    else
                    {
                        myFiles.WriteValue("Direct3d9", "adapter", theAvailableScreen.AdapterNumber);
                    }
                }
            }
            e.Handled = true;
        }

        #endregion

        #region Screen Resolution

        private void cboResolution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialising)
            {
                //grab the desired res
                ScreenResolution.ScreenRes theDesiredRes = cboResolution.SelectedItem as ScreenResolution.ScreenRes;

                if (theDesiredRes != null)
                {

                    //and add the correct attributes
                    IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");


                    //Refresh Rate
                    myFiles.WriteValue("Direct3d9", "fullscreenRefreshRate", theDesiredRes.Frequency);

                    //and screen dimensions
                    myFiles.WriteValue("ClientGraphics", "screenWidth", theDesiredRes.Width);
                    myFiles.WriteValue("ClientGraphics", "screenHeight", theDesiredRes.Height);
                }                
            }
            e.Handled = true;
        }

        #endregion

        #region Shader Version

        private void cboShaderVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialising)
            {
                ShaderVersion theShaderVersion = cboShaderVersion.SelectedItem as ShaderVersion;

                if (theShaderVersion != null)
                {
                    IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");

                    if (theShaderVersion.DeleteKeys)
                    {
                        myFiles.DeleteKey("Direct3d9", "maxVertexShaderVersion");
                        myFiles.DeleteKey("Direct3d9", "maxPixelShaderVersion");
                    }
                    else
                    {
                        myFiles.WriteValue("Direct3d9", "maxVertexShaderVersion", theShaderVersion.MaxVertexShaderVersion);
                        myFiles.WriteValue("Direct3d9", "maxPixelShaderVersion", theShaderVersion.MaxPixelShaderVersion);
                    }
                }               
            }
            e.Handled = true;
        }

        #endregion

        #endregion

        #region Sound

        #region Speakers
        private void cboSpeakers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialising)
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                myFiles.WriteValue("ClientAudio", "soundProvider", "\"" + (string)cboSpeakers.SelectedItem + "\"");
            }            
            e.Handled = true;
        }
        #endregion

        #endregion

        #region Game
        private void cboLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialising)
            {
                ClientLanguage theLanguage = cboLanguage.SelectedItem as ClientLanguage;

                if (theLanguage != null)
                {
                    IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");

                    myFiles.WriteValue("SharedGame", "defaultLocale", theLanguage.DefaultLocale);
                    myFiles.WriteValue("SharedGame", "fontLocale", theLanguage.FontLocale);
                }
            }           

            e.Handled = true;
        }

        #endregion

        #region Launcher

        #region Skin

        private void cboSkin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialising)
            {
                //and update the preferences                
                myPrefs.UpdateSettings(UserPreferences.SettingsType.Skin, cboSkin.SelectedItem.ToString());
                if (SettingsHaveChanged != null)
                {
                    SettingsHaveChanged(this, new SettingsChangedEventArgs(ChangedSetting.Skin, cboSkin.SelectedItem.ToString()));

                    //load the new pallettes as well
                    cboPallette.ItemsSource = theSettings.GetAvailablePallettes;
                    cboPallette.SelectedIndex = 0;                    
                }
            }
            e.Handled = true;
        }

        #endregion

        #region Pallette

        private void cboPallette_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialising)
            {
                //get the new pallette
                UISettings.Pallette theNewPallette = cboPallette.SelectedItem as UISettings.Pallette;

                if (theNewPallette != null)
                {
                    if (SettingsHaveChanged != null)
                    {
                        SettingsHaveChanged(this, new SettingsChangedEventArgs(ChangedSetting.Pallete, theNewPallette.Name));
                    }

                    //and update the preferences
                    myPrefs.UpdateSettings(UserPreferences.SettingsType.Pallette, theNewPallette.Name);
                }
            }
            e.Handled = true;
        }

        #endregion

        #region Star Wars Ghost Font

        private void cboGhostFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialising)
            {
                UISettings.StarWarsFont theFont = cboGhostFont.SelectedItem as UISettings.StarWarsFont;

                if (theFont != null)
                {
                    if (SettingsHaveChanged != null)
                    {
                        SettingsHaveChanged(this, new SettingsChangedEventArgs(ChangedSetting.GhostFont, theFont.Name));
                    }

                    //and update the preferences                   
                    myPrefs.UpdateSettings(UserPreferences.SettingsType.GhostFont, theFont.Name);
                }
            }
            e.Handled = true;

        }

        #endregion

        #endregion

        #endregion

        #region Shader Versions

        #region Item Wrapper Class

        public class ShaderVersion
        {
            public string DisplayText
            {
                get;
                set;
            }

            public string MaxVertexShaderVersion
            {
                get;
                set;
            }

            public string MaxPixelShaderVersion
            {
                get;
                set;
            }

            public bool DeleteKeys
            {
                get;
                set;
            }
        }

        #endregion

        #region Items Collection
        private List<ShaderVersion> lstShaderVersions;

        public List<ShaderVersion> ShaderVersions
        {
            get
            {
                if (lstShaderVersions == null)
                {
                    lstShaderVersions = new List<ShaderVersion>();

                    lstShaderVersions.Add(new ShaderVersion
                    {
                        DisplayText = "Disabled",
                        MaxVertexShaderVersion = "0",
                        MaxPixelShaderVersion = "0",                        
                        DeleteKeys = false
                    });

                    lstShaderVersions.Add(new ShaderVersion
                    {
                        DisplayText = "1.1 (Override)",
                        MaxVertexShaderVersion = "0x0101",
                        MaxPixelShaderVersion = "0x0101",                        
                        DeleteKeys = false
                    });

                    lstShaderVersions.Add(new ShaderVersion
                    {
                        DisplayText = "1.4 (Override)",
                        MaxVertexShaderVersion =  "0x0101",
                        MaxPixelShaderVersion = "0x0104",
                        DeleteKeys = false                         
                    });

                    lstShaderVersions.Add(new ShaderVersion
                    {
                        DisplayText = "2.0 (Override)",
                        MaxVertexShaderVersion = "0x0200",
                        MaxPixelShaderVersion = "0x0200",
                        DeleteKeys = false 
                    });

                    lstShaderVersions.Add(new ShaderVersion
                    {
                        DisplayText = "Optimal",
                        DeleteKeys = true,
                        MaxVertexShaderVersion = "",
                        MaxPixelShaderVersion = ""
                    });
                }

                return lstShaderVersions;
            }
        }

        #endregion

        #region Current Selection

        public ShaderVersion CurrentShaderVersion
        {
            get
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                List<KeyValuePair<string, string>> lstKeys = myFiles.GetSectionValuesAsList("Direct3d9");

                var maxVertexShaderVersion = lstKeys.Where(kvp => kvp.Key.Equals("maxVertexShaderVersion", StringComparison.InvariantCultureIgnoreCase));
                var maxPixelShaderVersion = lstKeys.Where(kvp => kvp.Key.Equals("maxPixelShaderVersion", StringComparison.InvariantCultureIgnoreCase));

                if ((maxPixelShaderVersion.Count() == 0) && (maxVertexShaderVersion.Count() == 0))
                {
                    return ShaderVersions.Where(sv => sv.DeleteKeys).Single();
                }
                else
                {
                    return ShaderVersions.Where(sv => 
                            sv.MaxPixelShaderVersion.Equals(maxPixelShaderVersion.First().Value, StringComparison.InvariantCultureIgnoreCase) 
                            && 
                            sv.MaxVertexShaderVersion.Equals(maxVertexShaderVersion.First().Value, StringComparison.InvariantCultureIgnoreCase)).Single();
                }
            }
        }

        #endregion

        #endregion

        #region Sound Modes

        public List<string> SoundVersions
        {
            get
            {
                List<string> lstSoundVersion = new List<string>();
                lstSoundVersion.Add("Windows Speaker Configuration");
                lstSoundVersion.Add("Headphones");
                lstSoundVersion.Add("2 Speakers");
                lstSoundVersion.Add("4 Speakers");
                lstSoundVersion.Add("5.1 Speakers");
                lstSoundVersion.Add("6.1 Speakers");
                lstSoundVersion.Add("7.1 Speakers");
                lstSoundVersion.Add("8.1 Speakers");
                lstSoundVersion.Add("Dolby Surround");

                return lstSoundVersion;
            }
        }

        #endregion      

        #region Languages

        #region Item Wrapper Class
        public class ClientLanguage
        {
            public string Name
            {
                get;
                set;
            }
            public string DefaultLocale
            {
                get;
                set;
            }
            public string FontLocale
            {
                get;
                set;
            }
        }

        #endregion

        #region Items Collection

        private List<ClientLanguage> lstLanguages;
        public List<ClientLanguage> Languages
        {
            get
            {
                if (lstLanguages == null)
                {

                    lstLanguages = new List<ClientLanguage>();

                    lstLanguages.Add(new ClientLanguage
                    {
                        Name = "English",
                        DefaultLocale = "en",
                        FontLocale = "en"
                    });

                    lstLanguages.Add(new ClientLanguage
                    {
                        Name = "日本語",
                        DefaultLocale = "ja",
                        FontLocale = "j5"
                    });
                }

                return lstLanguages;
            }
        }

        #endregion

        #region Current Selection

        public ClientLanguage CurrentLanguage
        {
            get
            {
                IniFiles myFiles = new IniFiles(SWGANHPAth + "swg2uu_opt.cfg");
                List<KeyValuePair<string, string>> lstKeys = myFiles.GetSectionValuesAsList("SharedGame");

                var defaultLocale = lstKeys.Where(kvp => kvp.Key.Equals("defaultLocale", StringComparison.InvariantCultureIgnoreCase));
                var fontLocale = lstKeys.Where(kvp => kvp.Key.Equals("fontLocale", StringComparison.InvariantCultureIgnoreCase));

                if ((defaultLocale.Count() == 0) && (fontLocale.Count() == 0))
                {
                    return Languages.Where(lang => lang.Name.Equals("English", StringComparison.InvariantCultureIgnoreCase)).Single();
                }
                else
                {
                    return Languages.Where(lang => 
                            lang.DefaultLocale.Equals(defaultLocale.First().Value, StringComparison.InvariantCultureIgnoreCase)
                            &&
                            lang.FontLocale.Equals(fontLocale.First().Value, StringComparison.InvariantCultureIgnoreCase)).Single();
                        
                }
            }
        }

        #endregion               

      
        #endregion          

        
    }
}
