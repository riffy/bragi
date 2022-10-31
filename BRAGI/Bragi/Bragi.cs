using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BRAGI.Bragi.Commands;
using OdinNative.Odin;
using OdinNative.Odin.Room;

namespace BRAGI.Bragi;

public enum DEVICETYPE
{
    IN,
    OUT
}
/// <summary>
/// Represents the state of Bragi, including Audio
/// </summary>
public enum BRAGISTATE
{
    NOT_INITIALIZED, // Audio Settings are missing
    INITIALIZED // Initial Audio Settings are set
}

public class Bragi
{
    public static event EventHandler? StateChanged;

    /// <summary>
    /// Refers to the current state of Bragi.
    /// If an ODIN client is present, it is considered initialized.
    /// </summary>
    /// 
    private BRAGISTATE _state = BRAGISTATE.NOT_INITIALIZED;
    public BRAGISTATE State 
    { 
        get 
        {
            return _state;
        } 
        set
        {
            _state = value;
            if (StateChanged != null) StateChanged.Invoke(this, null!);
        }
    }
    /// <summary>
    /// The currently running Bragi Instance
    /// </summary>
    public static Bragi? Instance { get; private set; }

    /// <summary>
    /// The acutal ODIN client
    /// </summary>
    /// 
    public static OdinClient? Client { get; set; }
    public Bragi() {
        if (Instance != null) throw new Exception();
        Instance = this;
    }
   
    /// <summary>
    /// Initializes the ODIN client using the defaults.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void Initialize()
    {
        if (State == BRAGISTATE.INITIALIZED) throw new Exception("ODIN Client already initialized.");
        Client = new OdinClient(OdinDefaults.Server, OdinDefaults.AccessKey);
        State = BRAGISTATE.INITIALIZED;
    }

    /// <summary>
    /// Performs a Cleanup sequence.
    /// Leaves all rooms the current ODIN client is connected.
    /// </summary>
    public void CleanUp()
    {
        BragiRoom.BragiRoom.CleanUp();
        State = BRAGISTATE.NOT_INITIALIZED;
        Client = null;
    }
}

/// <summary>
/// 
/// </summary>
public class BragiState
{
    public BRAGISTATE State { get; set; }

    public BragiState() { }
}
