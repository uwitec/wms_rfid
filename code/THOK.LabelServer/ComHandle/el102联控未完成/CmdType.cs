﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Zeng.ComfixtureHandle.el102
{
    enum CmdType
    {
        SendData,
        ResetElectronicLabel,
        ControlPlateID,
        OpenLight,
        CloseLight,
        LoudSpeaker,
        ShowOnly,
        KeyRaisedAck,
        Delete,
        ResetControlPlate,
        GetKey
    }
}
