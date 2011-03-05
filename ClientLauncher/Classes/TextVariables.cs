namespace ClientLauncher
{
    public class TextVariables
    {
        #region Private Variables
        private string _Name;
        private string _Locale;
        private string _FormTitle;
        private string _ShowHelp;
        private string _RefreshServers;
        private string _RefreshingServers;
        private string _Preferences;
        private string _GameClient;
        private string _Launcher;
        private string _Graphics;
        private string _DisplayAdapter;
        private string _GameResolution;
        private string _ShaderVersion;
        private string _WindowedMode;
        private string _BorderlessWindow;
        private string _DisableBumpMapping;
        private string _DisableHardwareMouseCursor;
        private string _UseLowDetailTextures;
        private string _UseLowDetailNormalMaps;
        private string _DisableMultiPassRendering;
        private string _DisableVSynch;
        private string _DisableFastMouseCursor;
        private string _UseSafeRenderer;
        private string _ContrainMouseCursor;
        private string _Sound;
        private string _DisableAudio;
        private string _MilesVersion;
        private string _Speakers;
        private string _Game;
        private string _SkipIntroSequence;
        private string _DisableCharSLOD;
        private string _ClientLanguage;
        private string _AllowMultipleInstances;
        private string _Advanced;
        private string _DisableWorldPreloading;
        private string _UseLowDetailCharacters;
        private string _UseLowDetailMeshes;
        private string _DisableTextureBaking;
        private string _DisableFileCaching;
        private string _DisableAsynchronousLoader;
        private string _ChooseGalaxy;
        private string _ServerNews;
        private string _ClientPatcher;
        private string _ChangeLanguage;
        private string _ServerName;
        private string _Description;
        private string _ServerBuild;
        private string _Population;
        private string _ServerInformation;
        private string _ManageCharacters;
        private string _Skin;
        private string _ColourScheme;
        private string _GhostFont;
        private string _Settings;
        private string _ViewFullArticle;
        private string _GameLauncher;
        private string _RegistryErrorOne;
        private string _RegistryErrorTwo;
        private string _Patching;
        private string _StandardFiles;
        private string _CustomFiles;
        private string _Server;
        private string _Username;
        private string _Password;
        private string _CharacterName;
        private string _DefaultServer;
        private string _NotConnected;
        private string _ChooseServerOne;
        private string _ChooseServerTwo;
        private string _Passed;
        private string _Failed;
        private string _Validating;
        private string _Found;
        private string _ValidatingFiles;
        private string _NotFound;
        private string _Complete;
        private string _SetupLocalConfig;
        private string _SettingServerAddress;
        private string _CopyConfigFiles;
        private string _CopyingBinaries;
        private string _CopyingSoundSystem;
        private string _Created;
        private string _CreatingDirectory;
        private string _CustomServerDir;
        private string _GettingCustomTre;
        private string _RSSError;
        private string _LoadingWait;
        private string _Loaded;
        private string _Items;
        private string _Connect;
        private string _SelectCharacter;
        private string _CustomServer;
        private string _IPAddress;
        private string _Port;
        private string _Add;
        private string _Cancel;  
        private string _UnableToDisplayRSS;
        private string _BlankUsername;
        private string _BlankPassword;
        private string _ClientUpToDate;
        private string _ClientOutOfDate;
        private string _PatchFromSWG;
        private string _ServerAvailable;
        private string _ConnectionTimeout;
        private string _Success;
        private string _CredentialsIncorrect;
        private string _ErrorLoading;
        private string _MultipleFiles;
        private string _LauncherNotSupportedOne;
        private string _LauncherNotSupportedTwo;
        private string _DownloadingUpdateOne;
        private string _DownloadingUpdateTwo;
        private string _RememberDetails;
        private string _AutoLogin;
        private string _Edit;
        private string _Delete;
        private string _ShowDebugMode;
        #endregion
        #region Public Accessors

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_Name))
                {
                    _Name = "English";
                }
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        public string Locale
        {
            get
            {
                if (string.IsNullOrEmpty(_Locale))
                {
                    _Locale = "en-gb";
                }
                return _Locale;
            }
            set
            {
                _Locale = value;
            }
        }
        public string FormTitle
        {
            get
            {
                if (string.IsNullOrEmpty(_FormTitle))
                {
                    _FormTitle = "SWG:ANH Client Launcher";
                }
                return _FormTitle;
            }
            set
            {
                _FormTitle = value;
            }
        }
        public string ShowHelp
        {
            get
            {
                if (string.IsNullOrEmpty(_ShowHelp))
                {
                    _ShowHelp = "Show Help";
                }
                return _ShowHelp;
            }
            set
            {
                _ShowHelp = value;
            }
        }
        public string RefreshServers
        {
            get
            {
                if (string.IsNullOrEmpty(_RefreshServers))
                {
                    _RefreshServers = "Refresh Servers";
                }
                return _RefreshServers;
            }
            set
            {
                _RefreshServers = value;
            }
        }
        public string RefreshingServers
        {
            get
            {
                if (string.IsNullOrEmpty(_RefreshingServers))
                {
                    _RefreshingServers = "Refreshing - Please Wait";
                }
                return _RefreshingServers;
            }
            set
            {
                _RefreshingServers = value;
            }
        }
        public string Preferences
        {
            get
            {
                if (string.IsNullOrEmpty(_Preferences))
                {
                    _Preferences = "Preferences";
                }
                return _Preferences;
            }
            set
            {
                _Preferences = value;
            }
        }
        public string GameClient
        {
            get
            {
                if (string.IsNullOrEmpty(_GameClient))
                {
                    _GameClient = "Game Client";
                }
                return _GameClient;
            }
            set
            {
                _GameClient = value;
            }
        }
        public string Launcher
        {
            get
            {
                if (string.IsNullOrEmpty(_Launcher))
                {
                    _Launcher = "Launcher";
                }
                return _Launcher;
            }
            set
            {
                _Launcher = value;
            }
        }
        public string Graphics
        {
            get
            {
                if (string.IsNullOrEmpty(_Graphics))
                {
                    _Graphics = "Graphics";
                }
                return _Graphics;
            }
            set
            {
                _Graphics = value;
            }
        }

        public string DisplayAdapter
        {
            get
            {
                if (string.IsNullOrEmpty(_DisplayAdapter))
                {
                    _DisplayAdapter = "Display Adapter";
                }
                return _DisplayAdapter;
            }
            set
            {
                _DisplayAdapter = value;
            }
        }
        public string GameResolution
        {
            get
            {
                if (string.IsNullOrEmpty(_GameResolution))
                {
                    _GameResolution = "Game Resolution";
                }
                return _GameResolution;
            }
            set
            {
                _GameResolution = value;
            }
        }
        public string ShaderVersion
        {
            get
            {
                if (string.IsNullOrEmpty(_ShaderVersion))
                {
                    _ShaderVersion = "Vertex/Pixel Shader Version";
                }
                return _ShaderVersion;
            }
            set
            {
                _ShaderVersion = value;
            }
        }
        public string WindowedMode
        {
            get
            {
                if (string.IsNullOrEmpty(_WindowedMode))
                {
                    _WindowedMode = "Windowed Mode";
                }
                return _WindowedMode;
            }
            set
            {
                _WindowedMode = value;
            }
        }
        public string BorderlessWindow
        {
            get
            {
                if (string.IsNullOrEmpty(_BorderlessWindow))
                {
                    _BorderlessWindow = "Borderless Window";
                }
                return _BorderlessWindow;
            }
            set
            {
                _BorderlessWindow = value;
            }
        }
        public string DisableBumpMapping
        {
            get
            {
                if (string.IsNullOrEmpty(_DisableBumpMapping))
                {
                    _DisableBumpMapping = "Disable Bump Mapping";
                }
                return _DisableBumpMapping;
            }
            set
            {
                _DisableBumpMapping = value;
            }
        }
        public string DisableHardwareMouseCursor
        {
            get
            {
                if (string.IsNullOrEmpty(_DisableHardwareMouseCursor))
                {
                    _DisableHardwareMouseCursor = "Disable Hardware Mouse Cursor";
                }
                return _DisableHardwareMouseCursor;
            }
            set
            {
                _DisableHardwareMouseCursor = value;
            }
        }
        public string UseLowDetailTextures
        {
            get
            {
                if (string.IsNullOrEmpty(_UseLowDetailTextures))
                {
                    _UseLowDetailTextures = "Use Low Detail Textures";
                }
                return _UseLowDetailTextures;
            }
            set
            {
                _UseLowDetailTextures = value;
            }
        }
        public string UseLowDetailNormalMaps
        {
            get
            {
                if (string.IsNullOrEmpty(_UseLowDetailNormalMaps))
                {
                    _UseLowDetailNormalMaps = "Use Low Detail Normal Maps";
                }
                return _UseLowDetailNormalMaps;
            }
            set
            {
                _UseLowDetailNormalMaps = value;
            }
        }
        public string DisableMultiPassRendering
        {
            get
            {
                if (string.IsNullOrEmpty(_DisableMultiPassRendering))
                {
                    _DisableMultiPassRendering = "Disable Multi Pass Rendering";
                }
                return _DisableMultiPassRendering;
            }
            set
            {
                _DisableMultiPassRendering = value;
            }
        }
        public string DisableVSynch
        {
            get
            {
                if (string.IsNullOrEmpty(_DisableVSynch))
                {
                    _DisableVSynch = "Disable VSynch (Allow Tearing)";
                }
                return _DisableVSynch;
            }
            set
            {
                _DisableVSynch = value;
            }
        }
        public string DisableFastMouseCursor
        {
            get
            {
                if (string.IsNullOrEmpty(_DisableFastMouseCursor))
                {
                    _DisableFastMouseCursor = "Disable Fast Mouse Cursor";
                }
                return _DisableFastMouseCursor;
            }
            set
            {
                _DisableFastMouseCursor = value;
            }
        }
        public string UseSafeRenderer
        {
            get
            {
                if (string.IsNullOrEmpty(_UseSafeRenderer))
                {
                    _UseSafeRenderer = "Use Safe Renderer (Slower)";
                }
                return _UseSafeRenderer;
            }
            set
            {
                _UseSafeRenderer = value;
            }
        }
        public string ContrainMouseCursor
        {
            get
            {
                if (string.IsNullOrEmpty(_ContrainMouseCursor))
                {
                    _ContrainMouseCursor = "Constrain Mouse Cursor to Window";
                }
                return _ContrainMouseCursor;
            }
            set
            {
                _ContrainMouseCursor = value;
            }
        }

        public string Sound
        {
            get
            {
                if (string.IsNullOrEmpty(_Sound))
                {
                    _Sound = "Sound";
                }
                return _Sound;
            }
            set
            {
                _Sound = value;
            }
        }
        public string DisableAudio
        {
            get
            {
                if (string.IsNullOrEmpty(_DisableAudio))
                {
                    _DisableAudio = "Disable Audio :";
                }
                return _DisableAudio;
            }
            set
            {
                _DisableAudio = value;
            }
        }
        public string MilesVersion
        {
            get
            {
                if (string.IsNullOrEmpty(_MilesVersion))
                {
                    _MilesVersion = "Miles Version :";
                }
                return _MilesVersion;
            }
            set
            {
                _MilesVersion = value;
            }
        }
        public string Speakers
        {
            get
            {
                if (string.IsNullOrEmpty(_Speakers))
                {
                    _Speakers = "Speakers :";
                }
                return _Speakers;
            }
            set
            {
                _Speakers = value;
            }
        }
        public string Game
        {
            get
            {
                if (string.IsNullOrEmpty(_Game))
                {
                    _Game = "Game";
                }
                return _Game;
            }
            set
            {
                _Game = value;
            }
        }

        public string AllowMultipleInstances
        {
            get
            {
                if (string.IsNullOrEmpty(_AllowMultipleInstances))
                {
                    _AllowMultipleInstances = "Allow Multiple Instances";
                }
                return _AllowMultipleInstances;
            }
            set
            {
                _AllowMultipleInstances = value;
            }

        }
        public string SkipIntroSequence
        {
            get
            {
                if (string.IsNullOrEmpty(_SkipIntroSequence))
                {
                    _SkipIntroSequence = "Skip Intro Sequence";
                }
                return _SkipIntroSequence;
            }
            set
            {
                _SkipIntroSequence = value;
            }
        }
        public string DisableCharSLOD
        {
            get
            {
                if (string.IsNullOrEmpty(_DisableCharSLOD))
                {
                    _DisableCharSLOD = "Disable Character System Level of Detail Manager";
                }
                return _DisableCharSLOD;
            }
            set
            {
                _DisableCharSLOD = value;
            }
        }
        public string ClientLanguage
        {
            get
            {
                if (string.IsNullOrEmpty(_ClientLanguage))
                {
                    _ClientLanguage = "Language :";
                }
                return _ClientLanguage;
            }
            set
            {
                _ClientLanguage = value;
            }
        }
        public string Advanced
        {
            get
            {
                if (string.IsNullOrEmpty(_Advanced))
                {
                    _Advanced = "Advanced";
                }
                return _Advanced;
            }
            set
            {
                _Advanced = value;
            }
        }
        public string DisableWorldPreloading
        {
            get
            {
                if (string.IsNullOrEmpty(_DisableWorldPreloading))
                {
                    _DisableWorldPreloading = "Disable World Preloading";
                }
                return _DisableWorldPreloading;
            }
            set
            {
                _DisableWorldPreloading = value;
            }
        }
        public string UseLowDetailCharacters
        {
            get
            {
                if (string.IsNullOrEmpty(_UseLowDetailCharacters))
                {
                    _UseLowDetailCharacters = "Use Low Detail Characters";
                }
                return _UseLowDetailCharacters;
            }
            set
            {
                _UseLowDetailCharacters = value;
            }
        }
        public string UseLowDetailMeshes
        {
            get
            {
                if (string.IsNullOrEmpty(_UseLowDetailMeshes))
                {
                    _UseLowDetailMeshes = "Use Low Detail Meshes";
                }
                return _UseLowDetailMeshes;
            }
            set
            {
                _UseLowDetailMeshes = value;
            }
        }
        public string DisableTextureBaking
        {
            get
            {
                if (string.IsNullOrEmpty(_DisableTextureBaking))
                {
                    _DisableTextureBaking = "Disable Texture Baking";
                }
                return _DisableTextureBaking;
            }
            set
            {
                _DisableTextureBaking = value;
            }
        }
        public string DisableFileCaching
        {
            get
            {
                if (string.IsNullOrEmpty(_DisableFileCaching))
                {
                    _DisableFileCaching = "Disable File Caching";
                }
                return _DisableFileCaching;
            }
            set
            {
                _DisableFileCaching = value;
            }
        }
        public string DisableAsynchronousLoader
        {
            get
            {
                if (string.IsNullOrEmpty(_DisableAsynchronousLoader))
                {
                    _DisableAsynchronousLoader = "Disable Asynchronous Loader";
                }
                return _DisableAsynchronousLoader;
            }
            set
            {
                _DisableAsynchronousLoader = value;
            }
        }
        public string ChooseGalaxy
        {
            get
            {
                if (string.IsNullOrEmpty(_ChooseGalaxy))
                {
                    _ChooseGalaxy = "Galaxy Selection";
                }
                return _ChooseGalaxy;
            }
            set
            {
                _ChooseGalaxy = value;
            }
        }
        public string ServerNews
        {
            get
            {
                if (string.IsNullOrEmpty(_ServerNews))
                {
                    _ServerNews = "Server News";
                }
                return _ServerNews;
            }
            set
            {
                _ServerNews = value;
            }
        }
        public string ClientPatcher
        {
            get
            {
                if (string.IsNullOrEmpty(_ClientPatcher))
                {
                    _ClientPatcher = "Client Patcher";
                }
                return _ClientPatcher;
            }
            set
            {
                _ClientPatcher = value;
            }
        }
        public string ChangeLanguage
        {
            get
            {
                if (string.IsNullOrEmpty(_ChangeLanguage))
                {
                    _ChangeLanguage = "Language Selection";
                }
                return _ChangeLanguage;
            }
            set
            {
                _ChangeLanguage = value;
            }
        }
        public string ServerName
        {
            get
            {
                if (string.IsNullOrEmpty(_ServerName))
                {
                    _ServerName = "Server Name :";
                }
                return _ServerName;
            }
            set
            {
                _ServerName = value;
            }
        }
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_Description))
                {
                    _Description = "Description :";
                }
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }
        public string ServerBuild
        {
            get
            {
                if (string.IsNullOrEmpty(_ServerBuild))
                {
                    _ServerBuild = "Build :";
                }
                return _ServerBuild;
            }
            set
            {
                _ServerBuild = value;
            }
        }
        public string Population
        {
            get
            {
                if (string.IsNullOrEmpty(_Population))
                {
                    _Population = "Population :";
                }
                return _Population;
            }
            set
            {
                _Population = value;
            }
        }
        public string ServerInformation
        {
            get
            {
                if (string.IsNullOrEmpty(_ServerInformation))
                {
                    _ServerInformation = "Server Information";
                }
                return _ServerInformation;
            }
            set
            {
                _ServerInformation = value;
            }
        }
        public string ManageCharacters
        {
            get
            {
                if (string.IsNullOrEmpty(_ManageCharacters))
                {
                    _ManageCharacters = "Manage Characters";
                }
                return _ManageCharacters;
            }
            set
            {
                _ManageCharacters = value;
            }
        }
        public string Skin
        {
            get
            {
                if (string.IsNullOrEmpty(_Skin))
                {
                    _Skin = "Skin :";
                }
                return _Skin;
            }
            set
            {
                _Skin = value;
            }
        }
        public string ColourScheme
        {
            get
            {
                if (string.IsNullOrEmpty(_ColourScheme))
                {
                    _ColourScheme = "Colour Scheme :";
                }
                return _ColourScheme;
            }
            set
            {
                _ColourScheme = value;
            }
        }
        public string GhostFont
        {
            get
            {
                if (string.IsNullOrEmpty(_GhostFont))
                {
                    _GhostFont = "Ghost Font Name :";
                }
                return _GhostFont;
            }
            set
            {
                _GhostFont = value;
            }
        }
        public string Settings
        {
            get
            {
                if (string.IsNullOrEmpty(_Settings))
                {
                    _Settings = "Client Settings";
                }
                return _Settings;
            }
            set
            {
                _Settings = value;
            }
        }


        public string ViewFullArticle
        {
            get
            {
                if (string.IsNullOrEmpty(_ViewFullArticle))
                {
                    _ViewFullArticle = "View Full Article";
                }
                return _ViewFullArticle;
            }
            set
            {
                _ViewFullArticle = value;
            }
        }

        public string GameLauncher
        {
            get
            {
                if (string.IsNullOrEmpty(_GameLauncher))
                {
                    _GameLauncher = "Launch SWG";
                }
                return _GameLauncher;
            }
            set
            {
                _GameLauncher = value;
            }
        }

        public string RegistryErrorOne
        {
            get
            {
                if (string.IsNullOrEmpty(_RegistryErrorOne))
                {
                    _RegistryErrorOne = "Unable to locate your SWG:ANH Client from the System Registry";
                }
                return _RegistryErrorOne;
            }
            set
            {
                _RegistryErrorOne = value;
            }
        }

        public string RegistryErrorTwo
        {
            get
            {
                if (string.IsNullOrEmpty(_RegistryErrorTwo))
                {
                    _RegistryErrorTwo = "Please point to the installation folder";
                }
                return _RegistryErrorTwo;
            }
            set
            {
                _RegistryErrorTwo = value;
            }
        }
        public string Patching
        {
            get
            {
                if (string.IsNullOrEmpty(_Patching))
                {
                    _Patching = "Checking custom files";
                }
                return _Patching;
            }
            set
            {
                _Patching = value;
            }
        }

        public string StandardFiles
        {
            get
            {
                if (string.IsNullOrEmpty(_StandardFiles))
                {
                    _StandardFiles = "Checking standard files";
                }
                return _StandardFiles;
            }
            set
            {
                _StandardFiles = value;
            }
        }
        public string CustomFiles
        {
            get
            {
                if (string.IsNullOrEmpty(_CustomFiles))
                {
                    _CustomFiles = "Checking custom files";
                }
                return _CustomFiles;
            }
            set
            {
                _CustomFiles = value;
            }
        }
        public string Server
        {
            get
            {
                if (string.IsNullOrEmpty(_Server))
                {
                    _Server = "Server";
                }
                return _Server;
            }
            set
            {
                _Server = value;
            }
        }
        public string Username
        {
            get
            {
                if (string.IsNullOrEmpty(_Username))
                {
                    _Username = "Username";
                }
                return _Username;
            }
            set
            {
                _Username = value;
            }
        }
        public string Password
        {
            get
            {
                if (string.IsNullOrEmpty(_Password))
                {
                    _Password = "Password";
                }
                return _Password;
            }
            set
            {
                _Password = value;
            }
        }
        public string CharacterName
        {
            get
            {
                if (string.IsNullOrEmpty(_CharacterName))
                {
                    _CharacterName = "Character Name";
                }
                return _CharacterName;
            }
            set
            {
                _CharacterName = value;
            }
        }
        public string DefaultServer
        {
            get
            {
                if (string.IsNullOrEmpty(_DefaultServer))
                {
                    _DefaultServer = "Default Server";
                }
                return _DefaultServer;
            }
            set
            {
                _DefaultServer = value;
            }
        }
        public string NotConnected
        {
            get
            {
                if (string.IsNullOrEmpty(_NotConnected))
                {
                    _NotConnected = "Not Connected : Cannot retrieve server list";
                }
                return _NotConnected;
            }
            set
            {
                _NotConnected = value;
            }
        }
        public string ChooseServerOne
        {
            get
            {
                if (string.IsNullOrEmpty(_ChooseServerOne))
                {
                    _ChooseServerOne = "You need to choose a server firstt";
                }
                return _ChooseServerOne;
            }
            set
            {
                _ChooseServerOne = value;
            }
        }
        public string ChooseServerTwo
        {
            get
            {
                if (string.IsNullOrEmpty(_ChooseServerTwo))
                {
                    _ChooseServerTwo = "Double click on a server to select it as default";
                }
                return _ChooseServerTwo;
            }
            set
            {
                _ChooseServerTwo = value;
            }
        }

        public string Passed
        {
            get
            {
                if (string.IsNullOrEmpty(_Passed))
                {
                    _Passed = "Passed";
                }
                return _Passed;
            }
            set
            {
                _Passed = value;
            }
        }

        public string Failed
        {
            get
            {
                if (string.IsNullOrEmpty(_Failed))
                {
                    _Failed = "Failed";
                }
                return _Failed;
            }
            set
            {
                _Failed = value;
            }
        }

        public string Validating
        {
            get
            {
                if (string.IsNullOrEmpty(_Validating))
                {
                    _Validating = "Validating";
                }
                return _Validating;
            }
            set
            {
                _Validating = value;
            }
        }

        public string Found
        {
            get
            {
                if (string.IsNullOrEmpty(_Found))
                {
                    _Found = "Found";
                }
                return _Found;
            }
            set
            {
                _Found = value;
            }
        }

        public string ValidatingFiles
        {
            get
            {
                if (string.IsNullOrEmpty(_ValidatingFiles))
                {
                    _ValidatingFiles = "Validating installed files";
                }
                return _ValidatingFiles;
            }
            set
            {
                _ValidatingFiles = value;
            }
        }

        public string Complete
        {
            get
            {
                if (string.IsNullOrEmpty(_Complete))
                {
                    _Complete = "Complete";
                }
                return _Complete;
            }
            set
            {
                _Complete = value;
            }
        }

        public string NotFound
        {
            get
            {
                if (string.IsNullOrEmpty(_NotFound))
                {
                    _NotFound = "Not Found";
                }
                return _NotFound;
            }
            set
            {
                _NotFound = value;
            }
        }

        public string SetupLocalConfig
        {
            get
            {
                if (string.IsNullOrEmpty(_SetupLocalConfig))
                {
                    _SetupLocalConfig = "Setting up local config for this server";
                }
                return _SetupLocalConfig;
            }
            set
            {
                _SetupLocalConfig = value;
            }
        }

        public string SettingServerAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_SettingServerAddress))
                {
                    _SettingServerAddress = "Setting server address";
                }
                return _SettingServerAddress;
            }
            set
            {
                _SettingServerAddress = value;
            }
        }

        public string CopyConfigFiles
        {
            get
            {
                if (string.IsNullOrEmpty(_CopyConfigFiles))
                {
                    _CopyConfigFiles = "Copying config files";
                }
                return _CopyConfigFiles;
            }
            set
            {
                _CopyConfigFiles = value;
            }
        }

        public string CopyingBinaries
        {
            get
            {
                if (string.IsNullOrEmpty(_CopyingBinaries))
                {
                    _CopyingBinaries = "Copying binaries";
                }
                return _CopyingBinaries;
            }
            set
            {
                _CopyingBinaries = value;
            }
        }


        public string CopyingSoundSystem
        {
            get
            {
                if (string.IsNullOrEmpty(_CopyingSoundSystem))
                {
                    _CopyingSoundSystem = "Copying Miles Sound Systems";
                }
                return _CopyingSoundSystem;
            }
            set
            {
                _CopyingSoundSystem = value;
            }
        }

        public string Created
        {
            get
            {
                if (string.IsNullOrEmpty(_Created))
                {
                    _Created = "Created";
                }
                return _Created;
            }
            set
            {
                _Created = value;
            }
        }

        public string CreatingDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_CreatingDirectory))
                {
                    _CreatingDirectory = "Creating directory";
                }
                return _CreatingDirectory;
            }
            set
            {
                _CreatingDirectory = value;
            }
        }

        public string CustomServerDir
        {
            get
            {
                if (string.IsNullOrEmpty(_CustomServerDir))
                {
                    _CustomServerDir = "Checking custom server directory";
                }
                return _CustomServerDir;
            }
            set
            {
                _CustomServerDir = value;
            }
        }


        public string GettingCustomTre
        {
            get
            {
                if (string.IsNullOrEmpty(_GettingCustomTre))
                {
                    _GettingCustomTre = "Getting Custom Tre List";
                }
                return _GettingCustomTre;
            }
            set
            {
                _GettingCustomTre = value;
            }
        }

        public string RSSError
        {
            get
            {
                if (string.IsNullOrEmpty(_RSSError))
                {
                    _RSSError = "Error Loading RSS";
                }
                return _RSSError;
            }
            set
            {
                _RSSError = value;
            }
        }

        public string LoadingWait
        {
            get
            {
                if (string.IsNullOrEmpty(_LoadingWait))
                {
                    _LoadingWait = "Loading results...please wait";
                }
                return _LoadingWait;
            }
            set
            {
                _LoadingWait = value;
            }
        }

        public string Loaded
        {
            get
            {
                if (string.IsNullOrEmpty(_Loaded))
                {
                    _Loaded = "Loaded";
                }
                return _Loaded;
            }
            set
            {
                _Loaded = value;
            }
        }

        public string Items
        {
            get
            {
                if (string.IsNullOrEmpty(_Items))
                {
                    _Items = "items";
                }
                return _Items;
            }
            set
            {
                _Items = value;
            }
        }

        public string Connect
        {
            get
            {
                if (string.IsNullOrEmpty(_Connect))
                {
                    _Connect = "Connect";
                }
                return _Connect;
            }
            set
            {
                _Connect = value;
            }
        }


        public string SelectCharacter
        {
            get
            {
                if (string.IsNullOrEmpty(_SelectCharacter))
                {
                    _SelectCharacter = "Select Character";
                }
                return _SelectCharacter;
            }
            set
            {
                _SelectCharacter = value;
            }
        }

        public string CustomServer
        {
            get
            {
                if (string.IsNullOrEmpty(_CustomServer))
                {
                    _CustomServer = "Custom Server";
                }
                return _CustomServer;
            }
            set
            {
                _CustomServer = value;
            }
        }

        public string IPAddress
        {
            get
            {
                if(string.IsNullOrEmpty(_IPAddress))
                {
                    _IPAddress = "IP Address";
                }
                return _IPAddress;
            }
            set
            {
                _IPAddress = value;
            }
        }

        public string Port
        {
            get
            {
                if(string.IsNullOrEmpty(_Port))
                {
                    _Port = "Port";
                }
                return _Port;
            }
            set
            {
                _Port = value;
            }
        }
        
        public string Add
        {
            get
            {
                if(string.IsNullOrEmpty(_Add))
                {
                    _Add = "Add";
                }
                return _Add;
            }
            set
            {
                _Add = value;
            }
        }

        public string Cancel
        {
            get
            {
                if(string.IsNullOrEmpty(_Cancel))
                {
                    _Cancel = "Cancel";
                }
                return _Cancel;
            }
            set
            {
                _Cancel = value;
            }
        }

        public string UnableToDisplayRSS
        {
            get
            {
                if(string.IsNullOrEmpty(_UnableToDisplayRSS))
                {
                    _UnableToDisplayRSS = "Unable to display RSS";
                }
                return _UnableToDisplayRSS;
            }
            set
            {
                _UnableToDisplayRSS = value;
            }
        }

        public string BlankUsername
        {
            get
            {
                if(string.IsNullOrEmpty(_BlankUsername))
                {
                    _BlankUsername = "Username is blank";
                }
                return _BlankUsername;
            }
            set
            {
                _BlankUsername = value;
            }
        }
        public string BlankPassword
        {
            get
            {
                if(string.IsNullOrEmpty(_BlankPassword))
                {
                    _BlankPassword = "Password is blank";
                }
                return _BlankPassword;
            }
            set
            {
                _BlankPassword = value;
            }
        }
        public string ClientUpToDate
        {
            get
            {
                if(string.IsNullOrEmpty(_ClientUpToDate))
                {
                    _ClientUpToDate = "Your client is up to date";
                }
                return _ClientUpToDate;
            }
            set
            {
                _ClientUpToDate = value;
            }
        }

        public string ClientOutOfDate
        {
            get
            {
                if(string.IsNullOrEmpty(_ClientOutOfDate))
                {
                    _ClientOutOfDate = "Your client needs updating";
                }
                return _ClientOutOfDate;
            }
            set
            {
                _ClientOutOfDate = value;
            }
        }  
        
        public string PatchFromSWG
        {
            get
            {
                if(string.IsNullOrEmpty(_PatchFromSWG))
                {
                    _PatchFromSWG = "Please patch your installion from the SWG Launchpad";
                }
                return _PatchFromSWG;
            }
            set
            {
                _PatchFromSWG = value;
            }
        } 

         public string ServerAvailable
        {
            get
            {
                if(string.IsNullOrEmpty(_ServerAvailable))
                {
                    _ServerAvailable = "Server is unavailable";
                }
                return _ServerAvailable;
            }
            set
            {
                _ServerAvailable = value;
            }
        } 

         public string ConnectionTimeout
        {
            get
            {
                if(string.IsNullOrEmpty(_ConnectionTimeout))
                {
                    _ConnectionTimeout = "Timed out when connecting to the server";
                }
                return _ConnectionTimeout;
            }
            set
            {
                _ConnectionTimeout = value;
            }
        } 

         public string Success
        {
            get
            {
                if(string.IsNullOrEmpty(_Success))
                {
                    _Success = "Success";
                }
                return _Success;
            }
            set
            {
                _Success = value;
            }
        }
         public string CredentialsIncorrect
        {
            get
            {
                if(string.IsNullOrEmpty(_CredentialsIncorrect))
                {
                    _CredentialsIncorrect = "Username or password incorrect";
                }
                return _CredentialsIncorrect;
            }
            set
            {
                _CredentialsIncorrect = value;
            }
        }  

         public string ErrorLoading
        {
            get
            {
                if(string.IsNullOrEmpty(_ErrorLoading))
                {
                    _ErrorLoading = "Error loading";
                }
                return _ErrorLoading;
            }
            set
            {
                _ErrorLoading = value;
            }
        }

         public string MultipleFiles
        {
            get
            {
                if(string.IsNullOrEmpty(_MultipleFiles))
                {
                    _MultipleFiles = "Multiple files found for";
                }
                return _MultipleFiles;
            }
            set
            {
                _MultipleFiles = value;
            }
        }
         public string LauncherNotSupportedOne
        {
            get
            {
                if(string.IsNullOrEmpty(_LauncherNotSupportedOne))
                {
                    _LauncherNotSupportedOne = "This server does not support connecting with this launcher";
                }
                return _LauncherNotSupportedOne;
            }
            set
            {
                _LauncherNotSupportedOne = value;
            }
        }
         public string LauncherNotSupportedTwo
        {
            get
            {
                if(string.IsNullOrEmpty(_LauncherNotSupportedTwo))
                {
                    _LauncherNotSupportedTwo = "Please connect via the normal login screeen or update LoginServer.exe to the latest build";
                }
                return _LauncherNotSupportedTwo;
            }
            set
            {
                _LauncherNotSupportedTwo = value;
            }
        }

         public string DownloadingUpdateOne
         {
             get
             {
                 if (string.IsNullOrEmpty(_DownloadingUpdateOne))
                 {
                     _DownloadingUpdateOne = "Downloading update";
                 }
                 return _DownloadingUpdateOne;
             }
             set
             {
                 _DownloadingUpdateOne = value;
             }
         }

         public string DownloadingUpdateTwo
         {
             get
             {
                 if (string.IsNullOrEmpty(_DownloadingUpdateTwo))
                 {
                     _DownloadingUpdateTwo = "Please wait";
                 }
                 return _DownloadingUpdateTwo;
             }
             set
             {
                 _DownloadingUpdateTwo = value;
             }
         }

         public string RememberDetails
         {
             get
             {
                 if (string.IsNullOrEmpty(_RememberDetails))
                 {
                     _RememberDetails = "Remember Details";
                 }
                 return _RememberDetails;
             }
             set
             {
                 _RememberDetails = value;
             }
         }
         public string AutoLogin
         {
             get
             {
                 if (string.IsNullOrEmpty(_AutoLogin))
                 {
                     _AutoLogin = "ogin Automatically";
                 }
                 return _AutoLogin;
             }
             set
             {
                 _AutoLogin = value;
             }
         }

         public string Edit
         {
             get
             {
                 if (string.IsNullOrEmpty(_Edit))
                 {
                     _Edit = "Edit";
                 }
                 return _Edit;
             }
             set
             {
                 _Edit = value;
             }
         }

         public string Delete
         {
             get
             {
                 if (string.IsNullOrEmpty(_Delete))
                 {
                     _Delete = "Delete";
                 }
                 return _Delete;
             }
             set
             {
                 _Delete = value;
             }
         }
         public string ShowDebugMode
         {
             get
             {
                 if (string.IsNullOrEmpty(_ShowDebugMode))
                 {
                     _ShowDebugMode = "Show debug mode";
                 }
                 return _ShowDebugMode;
             }
             set
             {
                 _ShowDebugMode = value;
             }
         }
 
        #endregion
    }
}

