using Godot;
using System;
using System.Threading.Tasks;

public partial class GDSync : Node
{
	private static Node GDSYNC;
	private static Node GDSYNC_SHARP;

	public static event Action Connected;
	public static event Action<int> ConnectionFailed; //error
	public static event Action Disconnected;
	public static event Action<int> ClientIdChanged; //error
	public static event Action<string> LobbyCreated;
	public static event Action<string, int> LobbyCreationFailed; //lobby name | error
	public static event Action<string> LobbyJoined; //lobby name
	public static event Action<string, int> LobbyJoinFailed; //lobby name | error
	public static event Action<string, Variant> LobbyDataChanged; //key | value
	public static event Action<string, Variant> LobbyTagChanged; //key | value
	public static event Action<int> ClientJoined; //client ID
	public static event Action<int> ClientLeft; //client ID
	public static event Action<int, string, Variant> PlayerDataChanged; //client ID | key | value
	public static event Action<Godot.Collections.Array> LobbiesReceived; //lobbies
	public static event Action<Godot.Collections.Dictionary> LobbyReceived; //lobby
	public static event Action<bool, int> HostChanged; //is host | host ID
	public static event Action<string, Godot.Collections.Array> SyncedEventTriggered; //event name | parameters
	public static event Action<string> ChangeSceneCalled; //scene path
	public static event Action<string> ChangeSceneSuccess; //scene path
	public static event Action<string> ChangeSceneFailed; //scene path
	public static event Action<string, bool> SteamJoinRequest; //lobby name | has password

	public override void _Ready()
	{
		GDSYNC = GetNode("/root/GDSync");
		GDSYNC_SHARP = this;

		GDSYNC.Connect("connected", Callable.From(() => { Connected?.Invoke(); }));
		GDSYNC.Connect("connection_failed", Callable.From((int error) => { ConnectionFailed?.Invoke(error); }));
		GDSYNC.Connect("disconnected", Callable.From(() => { Disconnected?.Invoke(); }));
		GDSYNC.Connect("client_id_changed", Callable.From((int ownID) => { ClientIdChanged?.Invoke(ownID); }));
		GDSYNC.Connect("lobby_created", Callable.From((string lobbyName) => { LobbyCreated?.Invoke(lobbyName); }));
		GDSYNC.Connect("lobby_creation_failed", Callable.From((string lobbyName, int error) => { LobbyCreationFailed?.Invoke(lobbyName, error); }));
		GDSYNC.Connect("lobby_joined", Callable.From((string lobbyName) => { LobbyJoined?.Invoke(lobbyName); }));
		GDSYNC.Connect("lobby_join_failed", Callable.From((string lobbyName, int error) => { LobbyJoinFailed?.Invoke(lobbyName, error); }));
		GDSYNC.Connect("lobby_data_changed", Callable.From((string key, Variant value) => { LobbyDataChanged?.Invoke(key, value); }));
		GDSYNC.Connect("lobby_tag_changed", Callable.From((string key, Variant value) => { LobbyTagChanged?.Invoke(key, value); }));
		GDSYNC.Connect("client_joined", Callable.From((int clientId) => { ClientJoined?.Invoke(clientId); }));
		GDSYNC.Connect("client_left", Callable.From((int clientId) => { ClientLeft?.Invoke(clientId); }));
		GDSYNC.Connect("player_data_changed", Callable.From((int clientId, string key, Variant value) => { PlayerDataChanged?.Invoke(clientId, key, value); }));
		GDSYNC.Connect("lobbies_received", Callable.From((Godot.Collections.Array lobbies) => { LobbiesReceived?.Invoke(lobbies); }));
		GDSYNC.Connect("lobby_received", Callable.From((Godot.Collections.Dictionary lobby) => { LobbyReceived?.Invoke(lobby); }));
		GDSYNC.Connect("host_changed", Callable.From((bool isHost, int hostID) => { HostChanged?.Invoke(isHost, hostID); }));
		GDSYNC.Connect("synced_event_triggered", Callable.From((string eventName, Godot.Collections.Array parameters) => { SyncedEventTriggered?.Invoke(eventName, parameters); }));
		GDSYNC.Connect("change_scene_called", Callable.From((string scenePath) => { ChangeSceneCalled?.Invoke(scenePath); }));
		GDSYNC.Connect("change_scene_success", Callable.From((string scenePath) => { ChangeSceneSuccess?.Invoke(scenePath); }));
		GDSYNC.Connect("change_scene_failed", Callable.From((string scenePath) => { ChangeSceneFailed?.Invoke(scenePath); }));
		GDSYNC.Connect("steam_join_request", Callable.From((string lobbyName, bool hasPassword) => { SteamJoinRequest?.Invoke(lobbyName, hasPassword); }));
	}




	// General functions -----------------------------------------------------------
	// *****************************************************************************
	// -----------------------------------------------------------------------------
	#region General Functions

	public static void StartMultiplayer()
	{
		GDSYNC.Call("start_multiplayer");
	}

	public static void StartLocalMultiplayer()
	{
		GDSYNC.Call("start_local_multiplayer");
	}

	public static void StopMultiplayer()
	{
		GDSYNC.Call("stop_multiplayer");
	}

	public static bool IsActive()
	{
		return (bool)GDSYNC.Call("is_active");
	}

	public static int GetClientId()
	{
		return (int)GDSYNC.Call("get_client_id");
	}

	public static int GetSenderId()
	{
		return (int)GDSYNC.Call("get_sender_id");
	}

	public static bool IsHost()
	{
		return (bool)GDSYNC.Call("is_host");
	}

	public static int GetHost()
	{
		return (int)GDSYNC.Call("get_host");
	}

	public static void SyncVar(Node node, string variableName, bool reliable = true)
	{
		GDSYNC.Call("sync_var", node, variableName, reliable);
	}

	public static void SyncVarOn(int clientId, Node node, string variableName, bool reliable = true)
	{
		GDSYNC.Call("sync_var_on", clientId, node, variableName, reliable);
	}

	public static void CallFunc(Callable callable, Godot.Collections.Array parameters = null, bool reliable = true)
	{
		GDSYNC.Call("call_func", callable, parameters, reliable);
	}

	public static void CallFuncOn(int clientId, Callable callable, Godot.Collections.Array parameters = null, bool reliable = true)
	{
		GDSYNC.Call("call_func_on", clientId, callable, parameters, reliable);
	}

	public static Node MultiplayerInstantiate(PackedScene scene, Node parent, bool syncStartingChanges = true, string[] excludedProperties = null, bool replicateOnJoin = true)
	{
		if (excludedProperties == null) excludedProperties = new string[0];
		return (Node)GDSYNC.Call("multiplayer_instantiate", scene, parent, syncStartingChanges, excludedProperties, replicateOnJoin);
	}

	public static void SyncedEventCreate(string eventName, float delay = 1.0f, Godot.Collections.Array parameters = null)
	{
		if (parameters == null) parameters = new Godot.Collections.Array();
		GDSYNC.Call("synced_event_create", eventName, delay, parameters);
	}

	public static void ChangeScene(string scenePath)
	{
		GDSYNC.Call("change_scene", scenePath);
	}
	#endregion






	// Security & safety functions -------------------------------------------------
	// *****************************************************************************
	// -----------------------------------------------------------------------------
	#region Security & Safety Functions

	public static void SetProtectionMode(bool protectionEnabled)
	{
		GDSYNC.Call("set_protection_mode", protectionEnabled);
	}

	public static void RegisterResource(Resource resource, string id)
	{
		GDSYNC.Call("register_resource", resource, id);
	}

	public static void DeregisterResource(Resource resource)
	{
		GDSYNC.Call("deregister_resource", resource);
	}

	public static void ExposeNode(Node node)
	{
		GDSYNC.Call("expose_node", node);
	}

	public static void HideNode(Node node)
	{
		GDSYNC.Call("hide_node", node);
	}

	public static void ExposeResource(Resource resource)
	{
		GDSYNC.Call("expose_resource", resource);
	}

	public static void HideResource(Resource resource)
	{
		GDSYNC.Call("hide_resource", resource);
	}

	public static void ExposeFunction(Callable callable)
	{
		GDSYNC.Call("expose_func", callable);
	}

	public static void HideFunction(Callable callable)
	{
		GDSYNC.Call("hide_function", callable);
	}

	public static void ExposeVar(Node node, string variableName)
	{
		GDSYNC.Call("expose_var", node, variableName);
	}

	public static void HideVar(Node node, string variableName)
	{
		GDSYNC.Call("hide_var", node, variableName);
	}
	#endregion


	// Node ownership --------------------------------------------------------------
	// *****************************************************************************
	// -----------------------------------------------------------------------------
	#region Node Ownership

	public static void SetGDSyncOwner(Node node, int owner)
	{
		GDSYNC.Call("set_gdsync_owner", node, owner);
	}

	public static void ClearGDSyncOwner(Node node)
	{
		GDSYNC.Call("clear_gdsync_owner", node);
	}

	public static int GetGDSyncOwner(Node node)
	{
		return (int)GDSYNC.Call("get_gdsync_owner", node);
	}

	public static bool IsGDSyncOwner(Node node)
	{
		return (bool)GDSYNC.Call("is_gdsync_owner", node);
	}

	public static void ConnectGDSyncOwnerChanged(Node node, Callable callable)
	{
		GDSYNC.Call("connect_gdsync_owner_changed", node, callable);
	}

	public static void DisconnectGDSyncOwnerChanged(Node node, Callable callable)
	{
		GDSYNC.Call("disconnect_gdsync_owner_changed", node, callable);
	}
	#endregion


	// Lobby functions -------------------------------------------------------------
	// *****************************************************************************
	// -----------------------------------------------------------------------------
	#region Lobby Functions

	public static void GetPublicLobbies()
	{
		GDSYNC.Call("get_public_lobbies");
	}

	public static void GetPublicLobby(string lobbyName)
	{
		GDSYNC.Call("get_public_lobby", lobbyName);
	}

	public static void LobbyCreate(string name, string password = "", bool isPublic = true, int playerLimit = 0, Godot.Collections.Dictionary tags = null, Godot.Collections.Dictionary data = null)
	{
		if (tags == null) tags = new Godot.Collections.Dictionary();
		if (data == null) data = new Godot.Collections.Dictionary();
		GDSYNC.Call("lobby_create", name, password, isPublic, playerLimit, tags, data);
	}

	public static void LobbyJoin(string name, string password = "")
	{
		GDSYNC.Call("lobby_join", name, password);
	}

	public static void LobbyClose()
	{
		GDSYNC.Call("lobby_close");
	}

	public static void LobbyOpen()
	{
		GDSYNC.Call("lobby_open");
	}

	public static void LobbySetVisibility(bool isPublic)
	{
		GDSYNC.Call("lobby_set_visibility", isPublic);
	}

	public static void LobbyChangePassword(string password)
	{
		GDSYNC.Call("lobby_change_password", password);
	}

	public static void LobbyLeave()
	{
		GDSYNC.Call("lobby_leave");
	}

	public static void LobbyKickClient(int clientId)
	{
		GDSYNC.Call("lobby_kick_client", clientId);
	}

	public static Godot.Collections.Array LobbyGetAllClients()
	{
		return (Godot.Collections.Array)GDSYNC.Call("lobby_get_all_clients");
	}

	public static int LobbyGetPlayerCount()
	{
		return (int)GDSYNC.Call("lobby_get_player_count");
	}

	public static string LobbyGetName()
	{
		return (string)GDSYNC.Call("lobby_get_name");
	}

	public static int LobbyGetPlayerLimit()
	{
		return (int)GDSYNC.Call("lobby_get_player_limit");
	}

	public static bool LobbyHasPassword()
	{
		return (bool)GDSYNC.Call("lobby_has_password");
	}

	public static void LobbySetTag(string key, Variant value)
	{
		GDSYNC.Call("lobby_set_tag", key, value);
	}

	public static void LobbyEraseTag(string key)
	{
		GDSYNC.Call("lobby_erase_tag", key);
	}

	public static bool LobbyHasTag(string key)
	{
		return (bool)GDSYNC.Call("lobby_has_tag", key);
	}

	public static Variant LobbyGetTag(string key, Variant defaultValue = new Variant())
	{
		return GDSYNC.Call("lobby_get_tag", key, defaultValue);
	}

	public static Godot.Collections.Dictionary LobbyGetAllTags()
	{
		return (Godot.Collections.Dictionary)GDSYNC.Call("lobby_get_all_tags");
	}

	public static void LobbySetData(string key, Variant value)
	{
		GDSYNC.Call("lobby_set_data", key, value);
	}

	public static void LobbyEraseData(string key)
	{
		GDSYNC.Call("lobby_erase_data", key);
	}

	public static bool LobbyHasData(string key)
	{
		return (bool)GDSYNC.Call("lobby_has_data", key);
	}

	public static Variant LobbyGetData(string key, Variant defaultValue = new Variant())
	{
		return GDSYNC.Call("lobby_get_data", key, defaultValue);
	}

	public static Godot.Collections.Dictionary LobbyGetAllData()
	{
		return (Godot.Collections.Dictionary)GDSYNC.Call("lobby_get_all_data");
	}
	#endregion

	// Player functions ------------------------------------------------------------
	// *****************************************************************************
	// -----------------------------------------------------------------------------
	#region Player Functions

	public static void PlayerSetData(string key, Variant value)
	{
		GDSYNC.Call("player_set_data", key, value);
	}

	public static void PlayerEraseData(string key)
	{
		GDSYNC.Call("player_erase_data", key);
	}

	public static Variant PlayerGetData(int clientId, string key, Variant defaultValue = new Variant())
	{
		return GDSYNC.Call("player_get_data", clientId, key, defaultValue);
	}

	public static Godot.Collections.Dictionary PlayerGetAllData(int clientId)
	{
		return (Godot.Collections.Dictionary)GDSYNC.Call("player_get_all_data", clientId);
	}

	public static void PlayerSetUsername(string username)
	{
		GDSYNC.Call("player_set_username", username);
	}
	#endregion

	// Accounts & Persistent Data Storage ------------------------------------------
	// *****************************************************************************
	// -----------------------------------------------------------------------------
	#region Accounts & Persistent Data Storage

	public static async Task<ACCOUNT_CREATION_RESPONSE_CODE> AccountCreate(string email, string username, string password)
	{
		var asyncRequest = GDSYNC.Call("account_create", email, username, password).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (ACCOUNT_CREATION_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<ACCOUNT_DELETION_RESPONSE_CODE> AccountDelete(string email, string password)
	{
		var asyncRequest = GDSYNC.Call("account_delete", email, password).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (ACCOUNT_DELETION_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<ACCOUNT_VERIFICATION_RESPONSE_CODE> AccountVerify(string email, string code, float validTime = 86400)
	{
		var asyncRequest = GDSYNC.Call("account_verify", email, code, validTime).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (ACCOUNT_VERIFICATION_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<ACCOUNT_RESEND_VERIFICATION_RESPONSE_CODE> AccountResendVerificationEmail(string email, string password)
	{
		var asyncRequest = GDSYNC.Call("account_resend_verification_code", email, password).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (ACCOUNT_RESEND_VERIFICATION_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<Godot.Collections.Dictionary> AccountIsVerified(string username = "")
	{
		var asyncRequest = GDSYNC.Call("account_is_verified", username).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (Godot.Collections.Dictionary)result[0];
	}

	public static async Task<Godot.Collections.Dictionary> AccountLogin(string email, string password, float validTime = 86400)
	{
		var asyncRequest = GDSYNC.Call("account_login", email, password, validTime).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (Godot.Collections.Dictionary)result[0];
	}

	public static async Task<Godot.Collections.Dictionary> AccountLoginFromSession(float validTime = 86400)
	{
		var asyncRequest = GDSYNC.Call("account_login_from_session", validTime).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (Godot.Collections.Dictionary)result[0];
	}

	public static async Task<ACCOUNT_LOGOUT_RESPONSE_CODE> AccountLogout()
	{
		var asyncRequest = GDSYNC.Call("account_logout").AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (ACCOUNT_LOGOUT_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<ACCOUNT_BAN_RESPONSE_CODE> AccountBan(float banDuration)
	{
		var asyncRequest = GDSYNC.Call("account_ban", banDuration).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (ACCOUNT_BAN_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<ACCOUNT_CHANGE_USERNAME_RESPONSE_CODE> AccountChangeAccountUsername(string newUsername)
	{
		var asyncRequest = GDSYNC.Call("account_change_username", newUsername).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (ACCOUNT_CHANGE_USERNAME_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<ACCOUNT_CHANGE_PASSWORD_RESPONSE_CODE> AccountChangeAccountPassword(string email, string password, string newPassword)
	{
		var asyncRequest = GDSYNC.Call("account_change_password", email, password, newPassword).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (ACCOUNT_CHANGE_PASSWORD_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<ACCOUNT_REQUEST_PASSWORD_RESET_RESPONSE_CODE> AccountRequestPasswordReset(string email)
	{
		var asyncRequest = GDSYNC.Call("account_request_password_reset", email).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (ACCOUNT_REQUEST_PASSWORD_RESET_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<ACCOUNT_RESET_PASSWORD_RESPONSE_CODE> AccountResetPassword(string email, string resetCode, string newPassword)
	{
		var asyncRequest = GDSYNC.Call("account_reset_password", email, resetCode, newPassword).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (ACCOUNT_RESET_PASSWORD_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<ACCOUNT_CREATE_REPORT_RESPONSE_CODE> AccountCreateReport(string usernameToReport, string report)
	{
		var asyncRequest = GDSYNC.Call("account_create_report", usernameToReport, report).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (ACCOUNT_CREATE_REPORT_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<ACCOUNT_SEND_FRIEND_REQUEST_RESPONSE_CODE> AccountSendFriendRequest(string friend)
	{
		var asyncRequest = GDSYNC.Call("account_send_friend_request", friend).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (ACCOUNT_SEND_FRIEND_REQUEST_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<ACCOUNT_ACCEPT_FRIEND_REQUEST_RESPONSE_CODE> AccountAcceptFriendRequest(string friend)
	{
		var asyncRequest = GDSYNC.Call("account_accept_friend_request", friend).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (ACCOUNT_ACCEPT_FRIEND_REQUEST_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<ACCOUNT_REMOVE_FRIEND_RESPONSE_CODE> AccountRemoveFriend(string friend)
	{
		var asyncRequest = GDSYNC.Call("account_remove_friend", friend).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (ACCOUNT_REMOVE_FRIEND_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<Godot.Collections.Dictionary> AccountGetFriendStatus(string friend)
	{
		var asyncRequest = GDSYNC.Call("account_get_friend_status", friend).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (Godot.Collections.Dictionary)result[0];
	}

	public static async Task<Godot.Collections.Dictionary> AccountGetFriends(string friend)
	{
		var asyncRequest = GDSYNC.Call("account_get_friends", friend).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (Godot.Collections.Dictionary)result[0];
	}

	public static async Task<ACCOUNT_DOCUMENT_SET_RESPONSE_CODE> AccountDocumentSet(string path, Godot.Collections.Dictionary document, bool externallyVisible = false)
	{
		var asyncRequest = GDSYNC.Call("account_document_set", path, document, externallyVisible).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (ACCOUNT_DOCUMENT_SET_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<ACCOUNT_DOCUMENT_SET_EXTERNAL_VISIBLE_RESPONSE_CODE> AccountDocumentSetExternalVisible(string path, bool externallyVisible = false)
	{
		var asyncRequest = GDSYNC.Call("account_document_set_external_visible", path, externallyVisible).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (ACCOUNT_DOCUMENT_SET_EXTERNAL_VISIBLE_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<Godot.Collections.Dictionary> AccountGetDocument(string path)
	{
		var asyncRequest = GDSYNC.Call("account_get_document", path).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (Godot.Collections.Dictionary)result[0];
	}

	public static async Task<Godot.Collections.Dictionary> AccountHasDocument(string path)
	{
		var asyncRequest = GDSYNC.Call("account_has_document", path).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (Godot.Collections.Dictionary)result[0];
	}

	public static async Task<Godot.Collections.Dictionary> AccountBrowseCollection(string path)
	{
		var asyncRequest = GDSYNC.Call("account_browse_collection", path).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (Godot.Collections.Dictionary)result[0];
	}

	public static async Task<ACCOUNT_DELETE_DOCUMENT_RESPONSE_CODE> AccountDeleteDocument(string path)
	{
		var asyncRequest = GDSYNC.Call("account_delete_document", path).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (ACCOUNT_DELETE_DOCUMENT_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<Godot.Collections.Dictionary> AccountGetExternalDocument(string externalUsername, string path)
	{
		var asyncRequest = GDSYNC.Call("account_get_external_document", externalUsername, path).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (Godot.Collections.Dictionary)result[0];
	}

	public static async Task<Godot.Collections.Dictionary> AccountHasExternalDocument(string externalUsername, string path)
	{
		var asyncRequest = GDSYNC.Call("account_has_external_document", externalUsername, path).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (Godot.Collections.Dictionary)result[0];
	}

	public static async Task<Godot.Collections.Dictionary> AccountBrowseExternalCollection(string externalUsername, string path)
	{
		var asyncRequest = GDSYNC.Call("account_browse_external_collection", externalUsername, path).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (Godot.Collections.Dictionary)result[0];
	}

	public static async Task<Godot.Collections.Dictionary> LeaderboardExists(string leaderboard)
	{
		var asyncRequest = GDSYNC.Call("leaderboard_exists", leaderboard).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (Godot.Collections.Dictionary)result[0];
	}

	public static async Task<Godot.Collections.Dictionary> LeaderboardGetAll()
	{
		var asyncRequest = GDSYNC.Call("leaderboard_get_all").AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (Godot.Collections.Dictionary)result[0];
	}

	public static async Task<Godot.Collections.Dictionary> LeaderboardBrowseScores(string leaderboard, int pageSize, int page)
	{
		var asyncRequest = GDSYNC.Call("leaderboard_browse_scores", leaderboard, pageSize, page).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (Godot.Collections.Dictionary)result[0];
	}

	public static async Task<Godot.Collections.Dictionary> LeaderboardGetScore(string leaderboard, string username)
	{
		var asyncRequest = GDSYNC.Call("leaderboard_get_score", leaderboard, username).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (Godot.Collections.Dictionary)result[0];
	}

	public static async Task<LEADERBOARD_SUBMIT_SCORE_RESPONSE_CODE> LeaderboardSubmitScore(string leaderboard, int score)
	{
		var asyncRequest = GDSYNC.Call("leaderboard_submit_score", leaderboard, score).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (LEADERBOARD_SUBMIT_SCORE_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<LEADERBOARD_DELETE_SCORE_RESPONSE_CODE> LeaderboardDeleteScore(string leaderboard)
	{
		var asyncRequest = GDSYNC.Call("leaderboard_delete_score", leaderboard).AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (LEADERBOARD_DELETE_SCORE_RESPONSE_CODE)(int)result[0];
	}
	#endregion




	#region Steam Integration
	public static async Task<bool> SteamIntegrationEnabled()
	{
		var asyncRequest = GDSYNC.Call("steam_integration_enabled").AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (bool)result[0];
	}

	public static async Task<LINK_STEAM_ACCOUNT_RESPONSE_CODE> SteamLinkAccount()
	{
		var asyncRequest = GDSYNC.Call("steam_link_account").AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (LINK_STEAM_ACCOUNT_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<UNLINK_STEAM_ACCOUNT_RESPONSE_CODE> SteamUnlinkAccount()
	{
		var asyncRequest = GDSYNC.Call("steam_unlink_account").AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (UNLINK_STEAM_ACCOUNT_RESPONSE_CODE)(int)result[0];
	}

	public static async Task<Godot.Collections.Dictionary> SteamLogin(float validTime = 86400)
	{
		var asyncRequest = GDSYNC.Call("steam_login").AsGodotObject();
		var result = await GDSYNC_SHARP.ToSignal(asyncRequest, "completed");
		return (Godot.Collections.Dictionary)result[0];
	}


	#endregion



	#region enums
	public enum CONNECTION_STATUS
	{
		LOBBY_SWITCH = -1,
		DISABLED,
		FINDING_LB,
		PINGING_SERVERS,
		CONNECTING,
		CONNECTED,
		CONNECTION_SECURED,
		LOCAL_CONNECTION
	}

	public enum PACKET_CHANNEL
	{
		SETUP,
		SERVER,
		RELIABLE,
		UNRELIABLE,
		INTERNAL,
	}

	public enum PACKET_VALUE
	{
		PADDING,
		CLIENT_REQUESTS,
		SERVER_REQUESTS,
		INTERNAL_REQUESTS,
	}

	public enum REQUEST_TYPE
	{
		VALIDATE_KEY,
		SECURE_CONNECTION,
		MESSAGE,
		SET_VARIABLE,
		CALL_FUNCTION,
		SET_VARIABLE_CACHED,
		CALL_FUNCTION_CACHED,
		CACHE_NODE_PATH,
		ERASE_NODE_PATH_CACHE,
		CACHE_NAME,
		ERASE_NAME_CACHE,
		SET_MC_OWNER,
		CREATE_LOBBY,
		JOIN_LOBBY,
		LEAVE_LOBBY,
		OPEN_LOBBY,
		CLOSE_LOBBY,
		SET_LOBBY_TAG,
		ERASE_LOBBY_TAG,
		SET_LOBBY_DATA,
		ERASE_LOBBY_DATA,
		SET_LOBBY_VISIBILITY,
		SET_LOBBY_PLAYER_LIMIT,
		SET_LOBBY_PASSWORD,
		GET_PUBLIC_LOBBIES,
		SET_PLAYER_USERNAME,
		SET_PLAYER_DATA,
		ERASE_PLAYER_DATA,
		SET_CONNECT_TIME,
		SET_SETTING,
		SET_CLIENT_ID,
		KICK_PLAYER,
		CHANGE_PASSWORD,
		GET_PUBLIC_LOBBY
	}

	public enum MESSAGE_TYPE
	{
		CRITICAL_ERROR,
		CLIENT_ID_RECEIVED,
		CLIENT_KEY_RECEIVED,
		INVALID_PUBLIC_KEY,
		SET_NODE_PATH_CACHE,
		ERASE_NODE_PATH_CACHE,
		SET_NAME_CACHE,
		ERASE_NAME_CACHE,
		SET_MC_OWNER,
		HOST_CHANGED,
		LOBBY_CREATED,
		LOBBY_CREATION_FAILED,
		LOBBY_JOINED,
		SWITCH_SERVER,
		LOBBY_JOIN_FAILED,
		LOBBIES_RECEIVED,
		LOBBY_DATA_RECEIVED,
		LOBBY_DATA_CHANGED,
		LOBBY_TAGS_CHANGED,
		PLAYER_DATA_RECEIVED,
		PLAYER_DATA_CHANGED,
		CLIENT_JOINED,
		CLIENT_LEFT,
		SET_CONNECT_TIME,
		SET_SENDER_ID,
		SET_DATA_USAGE,
		KICKED,
		LOBBY_RECEIVED
	}

	public enum SETTING
	{
		API_VERSION,
		USE_SENDER_ID,
	}

	public enum DATA
	{
		REQUEST_TYPE,
		NAME,
		VALUE,
		TARGET_CLIENT = 3,
	}

	public enum LOBBY_DATA
	{
		NAME = 1,
		PASSWORD = 2,
		PARAMETERS = 1,
		VISIBILITY = 1,
		VALUE = 2,
	}

	public enum FUNCTION_DATA
	{
		NODE_PATH = 1,
		NAME = 2,
		PARAMETERS = 4
	}

	public enum VAR_DATA
	{
		NODE_PATH = 1,
		NAME = 2,
		VALUE = 4
	}

	public enum MESSAGE_DATA
	{
		TYPE = 1,
		VALUE = 2,
		ERROR = 3,
		VALUE2 = 3,
	}

	public enum CRITICAL_ERROR
	{
		LOBBY_DATA_FULL,
		LOBBY_TAGS_FULL,
		PLAYER_DATA_FULL,
		REQUEST_TOO_LARGE,
	}

	public enum INTERNAL_MESSAGE
	{
		LOBBY_UPDATED,
		LOBBY_DELETED,
		REQUEST_LOBBIES,
		INCREASE_DATA_USAGE,
	}

	public enum CONNECTION_FAILED
	{
		INVALID_PUBLIC_KEY,
		TIMEOUT,
		LOCAL_PORT_ERROR,
	}

	public enum LOBBY_CREATION_ERROR
	{
		LOBBY_ALREADY_EXISTS,
		NAME_TOO_SHORT,
		NAME_TOO_LONG,
		PASSWORD_TOO_LONG,
		TAGS_TOO_LARGE,
		DATA_TOO_LARGE,
		ON_COOLDOWN,
		LOCAL_PORT_ERROR,
	}

	public enum LOBBY_JOIN_ERROR
	{
		LOBBY_DOES_NOT_EXIST,
		LOBBY_IS_CLOSED,
		LOBBY_IS_FULL,
		INCORRECT_PASSWORD,
		DUPLICATE_USERNAME,
	}

	public enum NODE_REPLICATION_SETTINGS
	{
		INSTANTIATOR,
		SYNC_STARTING_CHANGES,
		EXCLUDED_PROPERTIES,
		SCENE,
		TARGET,
		ORIGINAL_PROPERTIES,
	}

	public enum NODE_REPLICATION_DATA
	{
		ID,
		CHANGED_PROPERTIES,
	}

	public enum ACCOUNT_CREATION_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		STORAGE_FULL,
		INVALID_EMAIL,
		INVALID_USERNAME,
		EMAIL_ALREADY_EXISTS,
		USERNAME_ALREADY_EXISTS,
		USERNAME_TOO_SHORT,
		USERNAME_TOO_LONG,
		PASSWORD_TOO_SHORT,
		PASSWORD_TOO_LONG,
	}

	public enum ACCOUNT_DELETION_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		EMAIL_OR_PASSWORD_INCORRECT,
	}

	public enum ACCOUNT_RESEND_VERIFICATION_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		VERIFICATION_DISABLED,
		ON_COOLDOWN,
		ALREADY_VERIFIED,
		EMAIL_OR_PASSWORD_INCORRECT,
		BANNED,
	}

	public enum ACCOUNT_VERIFICATION_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		INCORRECT_CODE,
		CODE_EXPIRED,
		ALREADY_VERIFIED,
		BANNED,
	}

	public enum ACCOUNT_IS_VERIFIED_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		USER_DOESNT_EXIST,
	}

	public enum ACCOUNT_LOGIN_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		EMAIL_OR_PASSWORD_INCORRECT,
		NOT_VERIFIED,
		EXPIRED_SESSION,
		BANNED,
	}

	public enum ACCOUNT_LOGOUT_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
	}

	public enum ACCOUNT_CHANGE_PASSWORD_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		ON_COOLDOWN,
		EMAIL_OR_PASSWORD_INCORRECT,
		NOT_VERIFIED,
		BANNED,
	}

	public enum ACCOUNT_CHANGE_USERNAME_RESPONSE_CODE
	{

		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		ON_COOLDOWN,
		USERNAME_ALREADY_EXISTS,
		USERNAME_TOO_SHORT,
		USERNAME_TOO_LONG,
		INVALID_USERNAME
	}

	public enum ACCOUNT_RESET_PASSWORD_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		EMAIL_OR_CODE_INCORRECT,
		CODE_EXPIRED,
	}

	public enum ACCOUNT_REQUEST_PASSWORD_RESET_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		ON_COOLDOWN,
		EMAIL_DOESNT_EXIST,
		BANNED,
	}

	public enum ACCOUNT_DOCUMENT_SET_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		STORAGE_FULL,
	}

	public enum ACCOUNT_GET_DOCUMENT_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		DOESNT_EXIST,
	}

	public enum ACCOUNT_BROWSE_COLLECTION_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		DOESNT_EXIST,
	}

	public enum ACCOUNT_DOCUMENT_SET_EXTERNAL_VISIBLE_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		DOESNT_EXIST,
	}


	public enum ACCOUNT_HAS_DOCUMENT_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
	}

	public enum ACCOUNT_DELETE_DOCUMENT_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		DOESNT_EXIST,
	}

	public enum ACCOUNT_CREATE_REPORT_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		STORAGE_FULL,
		REPORT_TOO_LONG,
		TOO_MANY_REPORTS,
		USER_DOESNT_EXIST,
	}

	public enum LEADERBOARD_SUBMIT_SCORE_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		STORAGE_FULL,
		LEADERBOARD_DOESNT_EXIST
	}

	public enum LEADERBOARD_DELETE_SCORE_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		LEADERBOARD_DOESNT_EXIST
	}

	public enum LEADERBOARD_GET_ALL_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN
	}

	public enum LEADERBOARD_EXISTS_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN
	}

	public enum LEADERBOARD_BROWSE_SCORES_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		LEADERBOARD_DOESNT_EXIST
	}

	public enum LEADERBOARD_GET_SCORE_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		LEADERBOARD_DOESNT_EXIST,
		USER_DOESNT_EXIST
	}

	public enum ACCOUNT_SEND_FRIEND_REQUEST_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		STORAGE_FULL,
		USER_DOESNT_EXIST,
		FRIEND_ALREADY_ADDED,
		FRIENDS_LIST_FULL,
	}

	public enum ACCOUNT_ACCEPT_FRIEND_REQUEST_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		STORAGE_FULL,
		FRIEND_NOT_FOUND,
	}

	public enum ACCOUNT_REMOVE_FRIEND_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		STORAGE_FULL,
		FRIEND_DOESNT_EXIST,
	}

	public enum ACCOUNT_GET_FRIENDS_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
	}

	public enum ACCOUNT_GET_FRIEND_STATUS_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		USER_DOESNT_EXIST,
	}

	public enum FRIEND_STATUS
	{
		NONE,
		PENDING,
		FRIEND
	}

	public enum LINK_STEAM_ACCOUNT_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		ALREADY_LINKED,
		STEAM_ERROR,
	}

	public enum UNLINK_STEAM_ACCOUNT_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		STEAM_ERROR,
	}

	public enum STEAM_LOGIN_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		STEAM_ERROR,
		NOT_LINKED,
		NOT_VERIFIED,
		BANNED,
	}

	public enum ACCOUNT_BAN_RESPONSE_CODE
	{
		SUCCESS,
		NO_RESPONSE_FROM_SERVER,
		DATA_CAP_REACHED,
		RATE_LIMIT_EXCEEDED,
		NO_DATABASE,
		NOT_LOGGED_IN,
		STORAGE_FULL,
	}

	#endregion
}
