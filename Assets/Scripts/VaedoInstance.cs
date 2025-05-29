using UnityEngine;
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine.InputSystem;
using VeadoTube.BleatCan;
using Debug = UnityEngine.Debug;

public class VaedoInstance : MonoBehaviour, IInstancesReceiver, IConnectionReceiver
{
    const int PROCESS_WM_READ = 0x0010;
    
    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);
    
    public Instance instance;
    Connection connection;
    private Instances instances;

    private bool checkingClient = false;
    public TextMeshProUGUI HPText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;
        
    }

    // Example pointer chain: client.dll+0x6F070C, Offsets: 0x3C, 0x18, 0xCC, 0x1C4, 0xB8, 0x144, 0xB28

    void ReadHP()
    {
        if (Process.GetProcessesByName("bms").Length == 0)
            return;

        Process process = Process.GetProcessesByName("bms")[0];
        int index = -1;
        for (int i = 0; i < process.Modules.Count; i++)
        {
            Debug.Log(process.Modules[i].ModuleName + " - " + process.Modules[i].BaseAddress);
            if( process.Modules[i].ModuleName.Equals("engine.dll", StringComparison.OrdinalIgnoreCase)) 
            {
                index = i;
                break;
            }
        }
        if(index == -1)
        {
            Debug.LogError("engine.dll not found in process modules.");
            return;
        }
        // 1. Get base address of client.dll
        IntPtr baseAddress = process.Modules[index].BaseAddress;
        if (baseAddress == IntPtr.Zero)
            return;

        // 2. Add first offset to get the base pointer
        IntPtr address = IntPtr.Add(baseAddress, 0x009FA490);

        // 3. Open process for reading
        IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);
        byte[] buffer = new byte[4];
        int bytesRead = 0;

        // 4. Dereference pointer chain
        int[] offsets = { 0x26C, 0x10, 0x70, 0x0, 0x128, 0x24, 0x3D8};
        for (int i = 0; i < offsets.Length; i++)
        {
            // Read pointer at current address
            ReadProcessMemory((int)processHandle, address.ToInt32(), buffer, buffer.Length, ref bytesRead);
            address = (IntPtr)BitConverter.ToInt32(buffer, 0);
            if (address == IntPtr.Zero)
                return; // Invalid pointer chain

            // Add next offset, except after the last one
            if (i < offsets.Length - 1)
                address = IntPtr.Add(address, offsets[i]);
        }

        // 5. Read the HP value at the final address
        ReadProcessMemory((int)processHandle, IntPtr.Add(address, offsets[^1]).ToInt32(), buffer, buffer.Length, ref bytesRead);
        int hp = BitConverter.ToInt32(buffer, 0);

        HPText.text = hp.ToString();
        Debug.Log("HP: " + hp);
    }

    void MakeNewConnection()
    {
        instance = default;
        foreach (var i in Instances.Enumerate())
        {
            instance = i;
            Debug.Log("name: " + i.name);
        }
        if (!instance.id.isValid) return;
        instances = new Instances(this);
        connection = new Connection(instance.server, "Test", this);
        
        Debug.Log(connection.GetActive());
        
        Debug.Log("Checking client connection...");
        //StartCoroutine(CheckClient());
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            StartCoroutine(CheckClient());
        }
        
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            StartCoroutine(SendState("deer"));
        }
        
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            StartCoroutine(SendState("frog"));
        }
        
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            StartCoroutine(SendState("ew"));
        }
        
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            StartCoroutine(ClearStack());
        }
        
        ReadHP();
    }

    IEnumerator ClearStack()
    {
        MakeNewConnection();
        Debug.Log("Checking client connection...");
        while (connection.GetClient() == null)
        {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Client is now connected");

        JToken messageJson = new JObject
        {
            ["event"] = "payload",
            ["type"] = "stateEvents",
            ["id"] = "mini",
            ["payload"] = new JObject
            {
                ["event"] = "clear"
            }
        };
        
        byte[] messageBytes = messageJson.AsUTF8();
        bool sent = connection.Send("nodes", messageBytes);
        Debug.Log("Sent clear stack command: " + sent);
    }

    IEnumerator CheckClient()
    {
        MakeNewConnection();
        Debug.Log("Checking client connection...");
        while (connection.GetClient() == null)
        {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Client is now connected");
        
        JToken messageJson = new JObject
        {
            ["event"] = "payload",
            ["type"] = "stateEvents",
            ["id"] = "mini",
            ["payload"] = new JObject
            {
                ["event"] = "list"
            }
        };
        
        JToken booleanMessage = new JObject
        {
            ["event"] = "payload",
            ["type"] = "boolean",
            ["id"] = "mini",
            ["payload"] = new JObject
            {
                ["event"] = "get"
            }
        };
        byte[] messageBytes = messageJson.AsUTF8();
        bool sent = connection.Send("nodes", messageBytes);
        

        //string message = " stateEvents mini list";
        Debug.Log(connection.GetClient().State.ToString());
            
        //bool sent = connection.Send("nodes", Encoding.UTF8.GetBytes(message));
        Debug.Log("Sent: " + sent);
    }

    IEnumerator SendState(string state)
    {
        MakeNewConnection();
        Debug.Log("Checking client connection...");
        while (connection.GetClient() == null)
        {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Client is now connected");

        JToken messageJson = new JObject
        {
            ["event"] = "payload",
            ["type"] = "stateEvents",
            ["id"] = "mini",
            ["payload"] = new JObject
            {
                ["event"] = "set",
                ["state"] = state
            }
        };
        
        byte[] messageBytes = messageJson.AsUTF8();
        bool sent = connection.Send("nodes", messageBytes);
        Debug.Log($"Sent state: {state}, success: {sent}");
    }

    public void OnStart(Instance instance)
    {
        Debug.Log($"< started instance: {instance.id} ({instance.server})");
    }
    public void OnChange(Instance instance)
    {
        Debug.Log($"< changed instance: {instance.id} ({instance.server})");
    }
    public void OnEnd(InstanceID id)
    {
        Debug.Log($"< ended instance: {id}");
    }

    public void OnConnect(Connection connection, bool active)
    {
        Debug.Log($"connected: {active}");
    }

    public void OnError(Connection connection, ConnectionError error)
    {
        Debug.Log($"connection error: {error}");
    }
    public void OnReceive(Connection connection, string channel, byte[] data)
    {
        Debug.Log($"received: \"{channel}\"");
        try
        {
            Debug.Log(Encoding.UTF8.GetString(data));
        }
        catch
        {
            Debug.Log($"failed to parse");
        }
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct MODULEENTRY32
{
    public uint dwSize;
    public uint th32ModuleID;
    public uint th32ProcessID;
    public uint GlblcntUsage;
    public uint ProccntUsage;
    public IntPtr modBaseAddr;
    public uint modBaseSize;
    public IntPtr hModule;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string szModule;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string szExePath;
}

public class ModuleHelper
{
    const uint TH32CS_SNAPMODULE = 0x00000008;
    const uint TH32CS_SNAPMODULE32 = 0x00000010;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, int th32ProcessID);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

    public static IntPtr GetModuleBaseAddress(int processId, string moduleName)
    {
        IntPtr hSnap = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE | TH32CS_SNAPMODULE32, processId);
        if (hSnap == IntPtr.Zero) return IntPtr.Zero;

        MODULEENTRY32 me32 = new MODULEENTRY32();
        me32.dwSize = (uint)Marshal.SizeOf(typeof(MODULEENTRY32));
        if (Module32First(hSnap, ref me32))
        {
            do
            {
                if (me32.szModule.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
                {
                    return me32.modBaseAddr;
                }
            } while (Module32Next(hSnap, ref me32));
        }
        return IntPtr.Zero;
    }
}