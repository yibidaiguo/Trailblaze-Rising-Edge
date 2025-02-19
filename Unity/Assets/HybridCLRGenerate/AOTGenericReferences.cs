using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"JKFrame.dll",
		"System.Core.dll",
		"System.dll",
		"Unity.Addressables.dll",
		"Unity.Collections.dll",
		"Unity.Netcode.Runtime.dll",
		"Unity.ResourceManager.dll",
		"UnityEngine.CoreModule.dll",
		"UnityEngine.JSONSerializeModule.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// DelegateList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>>
	// DelegateList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
	// DelegateList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// DelegateList<float>
	// JKFrame.EventModule.MultipleParameterEventInfo<object>
	// JKFrame.ResSystem.<>c__DisplayClass21_0<object>
	// JKFrame.SingletonMono<object>
	// LocalizationConfigBase<int>
	// System.Action<GameSceneLanunchEvent>
	// System.Action<MouseActiveStateChangedEvent>
	// System.Action<MusicVolumeChangedEvent>
	// System.Action<SpawnMonsterEvent>
	// System.Action<SpawnPlayerEvent>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,object>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Action<UnityEngine.Vector2Int>
	// System.Action<float>
	// System.Action<int,int>
	// System.Action<int>
	// System.Action<object,object>
	// System.Action<object>
	// System.Action<ulong,object>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Vector2Int>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.Comparer<UnityEngine.Vector2Int>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<UnityEngine.Vector2Int,object>
	// System.Collections.Generic.Dictionary.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.Enumerator<ulong,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<UnityEngine.Vector2Int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<ulong,object>
	// System.Collections.Generic.Dictionary.KeyCollection<UnityEngine.Vector2Int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<byte,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<ulong,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<UnityEngine.Vector2Int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<ulong,object>
	// System.Collections.Generic.Dictionary.ValueCollection<UnityEngine.Vector2Int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<byte,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<ulong,object>
	// System.Collections.Generic.Dictionary<UnityEngine.Vector2Int,object>
	// System.Collections.Generic.Dictionary<byte,object>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.Dictionary<ulong,object>
	// System.Collections.Generic.EqualityComparer<UnityEngine.Vector2Int>
	// System.Collections.Generic.EqualityComparer<byte>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.EqualityComparer<ulong>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<UnityEngine.Vector2Int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<ulong,object>>
	// System.Collections.Generic.ICollection<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.ICollection<UnityEngine.Vector2Int>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.IComparer<UnityEngine.Vector2Int>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<UnityEngine.Vector2Int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ulong,object>>
	// System.Collections.Generic.IEnumerable<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.IEnumerable<UnityEngine.Vector2Int>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<UnityEngine.Vector2Int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<ulong,object>>
	// System.Collections.Generic.IEnumerator<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.IEnumerator<UnityEngine.Vector2Int>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<UnityEngine.Vector2Int>
	// System.Collections.Generic.IEqualityComparer<byte>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IEqualityComparer<ulong>
	// System.Collections.Generic.IList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.IList<UnityEngine.Vector2Int>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.IReadOnlyCollection<object>
	// System.Collections.Generic.KeyValuePair<UnityEngine.Vector2Int,object>
	// System.Collections.Generic.KeyValuePair<byte,object>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,int>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.KeyValuePair<ulong,object>
	// System.Collections.Generic.LinkedList.Enumerator<object>
	// System.Collections.Generic.LinkedList<object>
	// System.Collections.Generic.LinkedListNode<object>
	// System.Collections.Generic.List.Enumerator<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Vector2Int>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.List<UnityEngine.Vector2Int>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Vector2Int>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Vector2Int>
	// System.Collections.Generic.ObjectEqualityComparer<byte>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<ulong>
	// System.Collections.Generic.Queue.Enumerator<int>
	// System.Collections.Generic.Queue.Enumerator<object>
	// System.Collections.Generic.Queue<int>
	// System.Collections.Generic.Queue<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector2Int>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Comparison<UnityEngine.Vector2Int>
	// System.Comparison<object>
	// System.Func<UnityEngine.Bounds,byte>
	// System.Func<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
	// System.Func<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// System.Func<object,UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// System.Func<object,object>
	// System.Func<object>
	// System.IEquatable<Unity.Collections.FixedString32Bytes>
	// System.IEquatable<float>
	// System.Predicate<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Predicate<UnityEngine.Vector2Int>
	// System.Predicate<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<object>
	// System.Runtime.CompilerServices.TaskAwaiter<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Threading.Tasks.Task<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// System.Threading.Tasks.Task<object>
	// System.Threading.Tasks.TaskCompletionSource<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// System.Threading.Tasks.TaskCompletionSource<object>
	// Unity.Collections.IIndexable<byte>
	// Unity.Collections.INativeList<byte>
	// Unity.Netcode.BufferSerializer<Unity.Netcode.BufferSerializerReader>
	// Unity.Netcode.BufferSerializer<Unity.Netcode.BufferSerializerWriter>
	// Unity.Netcode.BufferSerializer<object>
	// Unity.Netcode.FallbackSerializer<Unity.Collections.FixedString32Bytes>
	// Unity.Netcode.FallbackSerializer<float>
	// Unity.Netcode.FallbackSerializer<int>
	// Unity.Netcode.FallbackSerializer<object>
	// Unity.Netcode.FixedStringSerializer<Unity.Collections.FixedString32Bytes>
	// Unity.Netcode.INetworkVariableSerializer<Unity.Collections.FixedString32Bytes>
	// Unity.Netcode.INetworkVariableSerializer<float>
	// Unity.Netcode.INetworkVariableSerializer<int>
	// Unity.Netcode.INetworkVariableSerializer<object>
	// Unity.Netcode.NetworkVariable.CheckExceedsDirtinessThresholdDelegate<Unity.Collections.FixedString32Bytes>
	// Unity.Netcode.NetworkVariable.CheckExceedsDirtinessThresholdDelegate<float>
	// Unity.Netcode.NetworkVariable.CheckExceedsDirtinessThresholdDelegate<int>
	// Unity.Netcode.NetworkVariable.CheckExceedsDirtinessThresholdDelegate<object>
	// Unity.Netcode.NetworkVariable.OnValueChangedDelegate<Unity.Collections.FixedString32Bytes>
	// Unity.Netcode.NetworkVariable.OnValueChangedDelegate<float>
	// Unity.Netcode.NetworkVariable.OnValueChangedDelegate<int>
	// Unity.Netcode.NetworkVariable.OnValueChangedDelegate<object>
	// Unity.Netcode.NetworkVariable<Unity.Collections.FixedString32Bytes>
	// Unity.Netcode.NetworkVariable<float>
	// Unity.Netcode.NetworkVariable<int>
	// Unity.Netcode.NetworkVariable<object>
	// Unity.Netcode.NetworkVariableSerialization.EqualsDelegate<Unity.Collections.FixedString32Bytes>
	// Unity.Netcode.NetworkVariableSerialization.EqualsDelegate<float>
	// Unity.Netcode.NetworkVariableSerialization.EqualsDelegate<int>
	// Unity.Netcode.NetworkVariableSerialization.EqualsDelegate<object>
	// Unity.Netcode.NetworkVariableSerialization<Unity.Collections.FixedString32Bytes>
	// Unity.Netcode.NetworkVariableSerialization<float>
	// Unity.Netcode.NetworkVariableSerialization<int>
	// Unity.Netcode.NetworkVariableSerialization<object>
	// Unity.Netcode.UnmanagedTypeSerializer<float>
	// Unity.Netcode.UnmanagedTypeSerializer<int>
	// Unity.Netcode.UserNetworkVariableSerialization.DuplicateValueDelegate<Unity.Collections.FixedString32Bytes>
	// Unity.Netcode.UserNetworkVariableSerialization.DuplicateValueDelegate<float>
	// Unity.Netcode.UserNetworkVariableSerialization.DuplicateValueDelegate<int>
	// Unity.Netcode.UserNetworkVariableSerialization.DuplicateValueDelegate<object>
	// Unity.Netcode.UserNetworkVariableSerialization.ReadDeltaDelegate<Unity.Collections.FixedString32Bytes>
	// Unity.Netcode.UserNetworkVariableSerialization.ReadDeltaDelegate<float>
	// Unity.Netcode.UserNetworkVariableSerialization.ReadDeltaDelegate<int>
	// Unity.Netcode.UserNetworkVariableSerialization.ReadDeltaDelegate<object>
	// Unity.Netcode.UserNetworkVariableSerialization.ReadValueDelegate<Unity.Collections.FixedString32Bytes>
	// Unity.Netcode.UserNetworkVariableSerialization.ReadValueDelegate<float>
	// Unity.Netcode.UserNetworkVariableSerialization.ReadValueDelegate<int>
	// Unity.Netcode.UserNetworkVariableSerialization.ReadValueDelegate<object>
	// Unity.Netcode.UserNetworkVariableSerialization.WriteDeltaDelegate<Unity.Collections.FixedString32Bytes>
	// Unity.Netcode.UserNetworkVariableSerialization.WriteDeltaDelegate<float>
	// Unity.Netcode.UserNetworkVariableSerialization.WriteDeltaDelegate<int>
	// Unity.Netcode.UserNetworkVariableSerialization.WriteDeltaDelegate<object>
	// Unity.Netcode.UserNetworkVariableSerialization.WriteValueDelegate<Unity.Collections.FixedString32Bytes>
	// Unity.Netcode.UserNetworkVariableSerialization.WriteValueDelegate<float>
	// Unity.Netcode.UserNetworkVariableSerialization.WriteValueDelegate<int>
	// Unity.Netcode.UserNetworkVariableSerialization.WriteValueDelegate<object>
	// Unity.Netcode.UserNetworkVariableSerialization<Unity.Collections.FixedString32Bytes>
	// Unity.Netcode.UserNetworkVariableSerialization<float>
	// Unity.Netcode.UserNetworkVariableSerialization<object>
	// UnityEngine.AddressableAssets.AddressablesImpl.<>c__DisplayClass79_0<object>
	// UnityEngine.Events.InvokableCall<float>
	// UnityEngine.Events.InvokableCall<object>
	// UnityEngine.Events.UnityAction<float>
	// UnityEngine.Events.UnityAction<object>
	// UnityEngine.Events.UnityEvent<float>
	// UnityEngine.Events.UnityEvent<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass60_0<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass60_0<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass61_0<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass61_0<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.<>c<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.<>c<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>
	// UnityEngine.ResourceManagement.ChainOperationTypelessDepedency<object>
	// UnityEngine.ResourceManagement.ResourceManager.CompletedOperation<object>
	// UnityEngine.ResourceManagement.Util.GlobalLinkedListNodeCache<object>
	// UnityEngine.ResourceManagement.Util.LinkedListNodeCache<object>
	// }}

	public void RefMethods()
	{
		// System.Void JKFrame.EventModule.AddEventListener<object>(string,object)
		// System.Void JKFrame.EventModule.AddMultipleParameterEventInfo<object>(string,object)
		// System.Void JKFrame.EventModule.EventTrigger<MouseActiveStateChangedEvent>(string,MouseActiveStateChangedEvent)
		// System.Void JKFrame.EventModule.EventTrigger<MusicVolumeChangedEvent>(string,MusicVolumeChangedEvent)
		// System.Void JKFrame.EventModule.EventTrigger<SpawnMonsterEvent>(string,SpawnMonsterEvent)
		// System.Void JKFrame.EventModule.EventTrigger<SpawnPlayerEvent>(string,SpawnPlayerEvent)
		// System.Void JKFrame.EventModule.RemoveEventListener<object>(string,object)
		// System.Void JKFrame.EventSystem.AddEventListener<GameSceneLanunchEvent>(string,System.Action<GameSceneLanunchEvent>)
		// System.Void JKFrame.EventSystem.AddEventListener<MouseActiveStateChangedEvent>(string,System.Action<MouseActiveStateChangedEvent>)
		// System.Void JKFrame.EventSystem.AddEventListener<MusicVolumeChangedEvent>(string,System.Action<MusicVolumeChangedEvent>)
		// System.Void JKFrame.EventSystem.AddEventListener<SpawnMonsterEvent>(string,System.Action<SpawnMonsterEvent>)
		// System.Void JKFrame.EventSystem.AddEventListener<SpawnPlayerEvent>(string,System.Action<SpawnPlayerEvent>)
		// System.Void JKFrame.EventSystem.AddTypeEventListener<GameSceneLanunchEvent>(System.Action<GameSceneLanunchEvent>)
		// System.Void JKFrame.EventSystem.AddTypeEventListener<MouseActiveStateChangedEvent>(System.Action<MouseActiveStateChangedEvent>)
		// System.Void JKFrame.EventSystem.AddTypeEventListener<MusicVolumeChangedEvent>(System.Action<MusicVolumeChangedEvent>)
		// System.Void JKFrame.EventSystem.AddTypeEventListener<SpawnMonsterEvent>(System.Action<SpawnMonsterEvent>)
		// System.Void JKFrame.EventSystem.AddTypeEventListener<SpawnPlayerEvent>(System.Action<SpawnPlayerEvent>)
		// System.Void JKFrame.EventSystem.EventTrigger<MouseActiveStateChangedEvent>(string,MouseActiveStateChangedEvent)
		// System.Void JKFrame.EventSystem.EventTrigger<MusicVolumeChangedEvent>(string,MusicVolumeChangedEvent)
		// System.Void JKFrame.EventSystem.EventTrigger<SpawnMonsterEvent>(string,SpawnMonsterEvent)
		// System.Void JKFrame.EventSystem.EventTrigger<SpawnPlayerEvent>(string,SpawnPlayerEvent)
		// System.Void JKFrame.EventSystem.RemoveTypeEventListener<MouseActiveStateChangedEvent>(System.Action<MouseActiveStateChangedEvent>)
		// System.Void JKFrame.EventSystem.RemoveTypeEventListener<MusicVolumeChangedEvent>(System.Action<MusicVolumeChangedEvent>)
		// System.Void JKFrame.EventSystem.RemoveTypeEventListener<SpawnPlayerEvent>(System.Action<SpawnPlayerEvent>)
		// System.Void JKFrame.EventSystem.TypeEventTrigger<MouseActiveStateChangedEvent>(MouseActiveStateChangedEvent)
		// System.Void JKFrame.EventSystem.TypeEventTrigger<MusicVolumeChangedEvent>(MusicVolumeChangedEvent)
		// System.Void JKFrame.EventSystem.TypeEventTrigger<SpawnMonsterEvent>(SpawnMonsterEvent)
		// System.Void JKFrame.EventSystem.TypeEventTrigger<SpawnPlayerEvent>(SpawnPlayerEvent)
		// object JKFrame.PoolSystem.GetGameObject<object>(string,UnityEngine.Transform)
		// object JKFrame.ResSystem.InstantiateGameObject<object>(UnityEngine.Transform,string,bool)
		// object JKFrame.ResSystem.InstantiateGameObject<object>(string,UnityEngine.Transform,string,bool)
		// System.Void JKFrame.ResSystem.InstantiateGameObjectAsync<object>(string,System.Action<object>,UnityEngine.Transform,string,bool)
		// object JKFrame.ResSystem.LoadAsset<object>(string)
		// System.Void JKFrame.UISystem.AddUIWindowData<object>(JKFrame.UIWindowData,bool)
		// System.Void JKFrame.UISystem.Close<object>()
		// object JKFrame.UISystem.GetWindow<object>()
		// object JKFrame.UISystem.Show<object>(int)
		// object LocalizationConfigBase<int>.GetContent<object>(string,int)
		// object LocalizationSystem.GetContent<object>(string,LanguageType)
		// object LocalizationSystem.GetContentByKey<object>(string,LanguageType)
		// AccountInfo System.Activator.CreateInstance<AccountInfo>()
		// C_S_AddTask System.Activator.CreateInstance<C_S_AddTask>()
		// C_S_BagSellItem System.Activator.CreateInstance<C_S_BagSellItem>()
		// C_S_BagSwapItem System.Activator.CreateInstance<C_S_BagSwapItem>()
		// C_S_BagUseItem System.Activator.CreateInstance<C_S_BagUseItem>()
		// C_S_ChatMessage System.Activator.CreateInstance<C_S_ChatMessage>()
		// C_S_CompleteDialogTask System.Activator.CreateInstance<C_S_CompleteDialogTask>()
		// C_S_CraftItem System.Activator.CreateInstance<C_S_CraftItem>()
		// C_S_Disonnect System.Activator.CreateInstance<C_S_Disonnect>()
		// C_S_EnterGame System.Activator.CreateInstance<C_S_EnterGame>()
		// C_S_GetBagData System.Activator.CreateInstance<C_S_GetBagData>()
		// C_S_GetTaskDatas System.Activator.CreateInstance<C_S_GetTaskDatas>()
		// C_S_Login System.Activator.CreateInstance<C_S_Login>()
		// C_S_Register System.Activator.CreateInstance<C_S_Register>()
		// C_S_ShopBuyItem System.Activator.CreateInstance<C_S_ShopBuyItem>()
		// C_S_ShortcutBarSetItem System.Activator.CreateInstance<C_S_ShortcutBarSetItem>()
		// C_S_ShortcutBarSwapItem System.Activator.CreateInstance<C_S_ShortcutBarSwapItem>()
		// S_C_AddTask System.Activator.CreateInstance<S_C_AddTask>()
		// S_C_BagUpdateItem System.Activator.CreateInstance<S_C_BagUpdateItem>()
		// S_C_ChatMessage System.Activator.CreateInstance<S_C_ChatMessage>()
		// S_C_CompleteTask System.Activator.CreateInstance<S_C_CompleteTask>()
		// S_C_Disonnect System.Activator.CreateInstance<S_C_Disonnect>()
		// S_C_GetBagData System.Activator.CreateInstance<S_C_GetBagData>()
		// S_C_GetTaskDatas System.Activator.CreateInstance<S_C_GetTaskDatas>()
		// S_C_Login System.Activator.CreateInstance<S_C_Login>()
		// S_C_Register System.Activator.CreateInstance<S_C_Register>()
		// S_C_ShortcutBarUpdateItem System.Activator.CreateInstance<S_C_ShortcutBarUpdateItem>()
		// S_C_UpdateCoinCount System.Activator.CreateInstance<S_C_UpdateCoinCount>()
		// S_C_UpdateTask System.Activator.CreateInstance<S_C_UpdateTask>()
		// Unity.Collections.FixedString32Bytes System.Activator.CreateInstance<Unity.Collections.FixedString32Bytes>()
		// object System.Activator.CreateInstance<object>()
		// object[] System.Array.Empty<object>()
		// int System.Enum.Parse<int>(string)
		// int System.Enum.Parse<int>(string,bool)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf<int>(int&)
		// System.Void Unity.Netcode.BufferSerializer<object>.SerializeValue<byte>(byte&,Unity.Netcode.FastBufferWriter.ForEnums)
		// System.Void Unity.Netcode.BufferSerializer<object>.SerializeValue<byte>(byte&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.BufferSerializer<object>.SerializeValue<int>(int&,Unity.Netcode.FastBufferWriter.ForEnums)
		// System.Void Unity.Netcode.BufferSerializer<object>.SerializeValue<int>(int&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.BufferSerializer<object>.SerializeValue<int>(int[]&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.BufferSerializer<object>.SerializeValue<object>(object&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<AccountInfo>(AccountInfo&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<C_S_AddTask>(C_S_AddTask&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<C_S_BagSellItem>(C_S_BagSellItem&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<C_S_BagSwapItem>(C_S_BagSwapItem&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<C_S_BagUseItem>(C_S_BagUseItem&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<C_S_ChatMessage>(C_S_ChatMessage&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<C_S_CompleteDialogTask>(C_S_CompleteDialogTask&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<C_S_CraftItem>(C_S_CraftItem&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<C_S_Disonnect>(C_S_Disonnect&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<C_S_EnterGame>(C_S_EnterGame&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<C_S_GetBagData>(C_S_GetBagData&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<C_S_GetTaskDatas>(C_S_GetTaskDatas&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<C_S_Login>(C_S_Login&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<C_S_Register>(C_S_Register&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<C_S_ShopBuyItem>(C_S_ShopBuyItem&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<C_S_ShortcutBarSetItem>(C_S_ShortcutBarSetItem&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<C_S_ShortcutBarSwapItem>(C_S_ShortcutBarSwapItem&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<S_C_AddTask>(S_C_AddTask&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<S_C_BagUpdateItem>(S_C_BagUpdateItem&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<S_C_ChatMessage>(S_C_ChatMessage&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<S_C_CompleteTask>(S_C_CompleteTask&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<S_C_Disonnect>(S_C_Disonnect&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<S_C_GetBagData>(S_C_GetBagData&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<S_C_GetTaskDatas>(S_C_GetTaskDatas&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<S_C_Login>(S_C_Login&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<S_C_Register>(S_C_Register&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<S_C_ShortcutBarUpdateItem>(S_C_ShortcutBarUpdateItem&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<S_C_UpdateCoinCount>(S_C_UpdateCoinCount&)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<S_C_UpdateTask>(S_C_UpdateTask&)
		// System.Void Unity.Netcode.FastBufferReader.ReadUnmanagedSafe<byte>(byte&)
		// System.Void Unity.Netcode.FastBufferReader.ReadUnmanagedSafe<float>(float&)
		// System.Void Unity.Netcode.FastBufferReader.ReadUnmanagedSafe<int>(int&)
		// System.Void Unity.Netcode.FastBufferReader.ReadUnmanagedSafe<object>(object&)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<AccountInfo>(AccountInfo&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<C_S_AddTask>(C_S_AddTask&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<C_S_BagSellItem>(C_S_BagSellItem&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<C_S_BagSwapItem>(C_S_BagSwapItem&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<C_S_BagUseItem>(C_S_BagUseItem&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<C_S_ChatMessage>(C_S_ChatMessage&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<C_S_CompleteDialogTask>(C_S_CompleteDialogTask&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<C_S_CraftItem>(C_S_CraftItem&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<C_S_Disonnect>(C_S_Disonnect&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<C_S_EnterGame>(C_S_EnterGame&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<C_S_GetBagData>(C_S_GetBagData&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<C_S_GetTaskDatas>(C_S_GetTaskDatas&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<C_S_Login>(C_S_Login&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<C_S_Register>(C_S_Register&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<C_S_ShopBuyItem>(C_S_ShopBuyItem&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<C_S_ShortcutBarSetItem>(C_S_ShortcutBarSetItem&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<C_S_ShortcutBarSwapItem>(C_S_ShortcutBarSwapItem&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<S_C_AddTask>(S_C_AddTask&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<S_C_BagUpdateItem>(S_C_BagUpdateItem&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<S_C_ChatMessage>(S_C_ChatMessage&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<S_C_CompleteTask>(S_C_CompleteTask&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<S_C_Disonnect>(S_C_Disonnect&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<S_C_GetBagData>(S_C_GetBagData&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<S_C_GetTaskDatas>(S_C_GetTaskDatas&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<S_C_Login>(S_C_Login&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<S_C_Register>(S_C_Register&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<S_C_ShortcutBarUpdateItem>(S_C_ShortcutBarUpdateItem&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<S_C_UpdateCoinCount>(S_C_UpdateCoinCount&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<S_C_UpdateTask>(S_C_UpdateTask&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<Unity.Collections.FixedString32Bytes>(Unity.Collections.FixedString32Bytes&,Unity.Netcode.FastBufferWriter.ForFixedStrings)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<byte>(byte&,Unity.Netcode.FastBufferWriter.ForEnums)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<byte>(byte&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<float>(float&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<int>(int&,Unity.Netcode.FastBufferWriter.ForEnums)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<object>(object&,Unity.Netcode.FastBufferWriter.ForEnums)
		// System.Void Unity.Netcode.FastBufferWriter.WriteNetworkSerializable<C_S_AddTask>(C_S_AddTask&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteNetworkSerializable<C_S_BagSellItem>(C_S_BagSellItem&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteNetworkSerializable<C_S_BagSwapItem>(C_S_BagSwapItem&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteNetworkSerializable<C_S_BagUseItem>(C_S_BagUseItem&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteNetworkSerializable<C_S_ChatMessage>(C_S_ChatMessage&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteNetworkSerializable<C_S_CompleteDialogTask>(C_S_CompleteDialogTask&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteNetworkSerializable<C_S_CraftItem>(C_S_CraftItem&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteNetworkSerializable<C_S_EnterGame>(C_S_EnterGame&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteNetworkSerializable<C_S_GetBagData>(C_S_GetBagData&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteNetworkSerializable<C_S_GetTaskDatas>(C_S_GetTaskDatas&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteNetworkSerializable<C_S_Login>(C_S_Login&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteNetworkSerializable<C_S_Register>(C_S_Register&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteNetworkSerializable<C_S_ShopBuyItem>(C_S_ShopBuyItem&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteNetworkSerializable<C_S_ShortcutBarSetItem>(C_S_ShortcutBarSetItem&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteNetworkSerializable<C_S_ShortcutBarSwapItem>(C_S_ShortcutBarSwapItem&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteNetworkSerializable<object>(object&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteUnmanaged<int>(int&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteUnmanagedSafe<byte>(byte&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteUnmanagedSafe<float>(float&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteUnmanagedSafe<int>(int&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteUnmanagedSafe<object>(object&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValue<Unity.Collections.FixedString32Bytes>(Unity.Collections.FixedString32Bytes&,Unity.Netcode.FastBufferWriter.ForFixedStrings)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<C_S_AddTask>(C_S_AddTask&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<C_S_BagSellItem>(C_S_BagSellItem&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<C_S_BagSwapItem>(C_S_BagSwapItem&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<C_S_BagUseItem>(C_S_BagUseItem&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<C_S_ChatMessage>(C_S_ChatMessage&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<C_S_CompleteDialogTask>(C_S_CompleteDialogTask&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<C_S_CraftItem>(C_S_CraftItem&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<C_S_EnterGame>(C_S_EnterGame&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<C_S_GetBagData>(C_S_GetBagData&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<C_S_GetTaskDatas>(C_S_GetTaskDatas&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<C_S_Login>(C_S_Login&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<C_S_Register>(C_S_Register&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<C_S_ShopBuyItem>(C_S_ShopBuyItem&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<C_S_ShortcutBarSetItem>(C_S_ShortcutBarSetItem&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<C_S_ShortcutBarSwapItem>(C_S_ShortcutBarSwapItem&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<Unity.Collections.FixedString32Bytes>(Unity.Collections.FixedString32Bytes&,Unity.Netcode.FastBufferWriter.ForFixedStrings)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<byte>(byte&,Unity.Netcode.FastBufferWriter.ForEnums)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<byte>(byte&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<float>(float&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<int>(int&,Unity.Netcode.FastBufferWriter.ForEnums)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<object>(object&,Unity.Netcode.FastBufferWriter.ForEnums)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<object>(object&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.INetworkSerializable.NetworkSerialize<Unity.Netcode.BufferSerializerReader>(Unity.Netcode.BufferSerializer<Unity.Netcode.BufferSerializerReader>)
		// System.Void Unity.Netcode.INetworkSerializable.NetworkSerialize<Unity.Netcode.BufferSerializerWriter>(Unity.Netcode.BufferSerializer<Unity.Netcode.BufferSerializerWriter>)
		// System.Void Unity.Netcode.IReaderWriter.SerializeValue<byte>(byte&,Unity.Netcode.FastBufferWriter.ForEnums)
		// System.Void Unity.Netcode.IReaderWriter.SerializeValue<byte>(byte&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.IReaderWriter.SerializeValue<int>(int&,Unity.Netcode.FastBufferWriter.ForEnums)
		// System.Void Unity.Netcode.IReaderWriter.SerializeValue<int>(int&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.IReaderWriter.SerializeValue<int>(int[]&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.IReaderWriter.SerializeValue<object>(object&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// bool Unity.Netcode.NetworkVariableSerialization<Unity.Collections.FixedString32Bytes>.EqualityEquals<Unity.Collections.FixedString32Bytes>(Unity.Collections.FixedString32Bytes&,Unity.Collections.FixedString32Bytes&)
		// bool Unity.Netcode.NetworkVariableSerialization<float>.EqualityEquals<float>(float&,float&)
		// bool Unity.Netcode.NetworkVariableSerialization<int>.ValueEquals<int>(int&,int&)
		// System.Void Unity.Netcode.NetworkVariableSerializationTypes.InitializeEqualityChecker_UnmanagedIEquatable<Unity.Collections.FixedString32Bytes>()
		// System.Void Unity.Netcode.NetworkVariableSerializationTypes.InitializeEqualityChecker_UnmanagedIEquatable<float>()
		// System.Void Unity.Netcode.NetworkVariableSerializationTypes.InitializeEqualityChecker_UnmanagedValueEquals<int>()
		// System.Void Unity.Netcode.NetworkVariableSerializationTypes.InitializeSerializer_FixedString<Unity.Collections.FixedString32Bytes>()
		// System.Void Unity.Netcode.NetworkVariableSerializationTypes.InitializeSerializer_UnmanagedByMemcpy<float>()
		// System.Void Unity.Netcode.NetworkVariableSerializationTypes.InitializeSerializer_UnmanagedByMemcpy<int>()
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<object>(object)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetAsync<object>(object)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetWithChain<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,object)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.AddressablesImpl.TrackHandle<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>)
		// object UnityEngine.Component.GetComponent<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>(bool)
		// bool UnityEngine.Component.TryGetComponent<object>(object&)
		// object UnityEngine.GameObject.GetComponent<object>()
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
		// bool UnityEngine.GameObject.TryGetComponent<object>(object&)
		// object UnityEngine.JsonUtility.FromJson<object>(string)
		// object UnityEngine.Object.Instantiate<object>(object)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.Convert<object>()
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateChainOperation<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,System.Func<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateCompletedOperationInternal<object>(object,bool,System.Exception,bool)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateCompletedOperationWithException<object>(object,System.Exception)
		// object UnityEngine.ResourceManagement.ResourceManager.CreateOperation<object>(System.Type,int,UnityEngine.ResourceManagement.Util.IOperationCacheKey,System.Action<UnityEngine.ResourceManagement.AsyncOperations.IAsyncOperation>)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.ProvideResource<object>(UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.StartOperation<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase<object>,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle)
	}
}