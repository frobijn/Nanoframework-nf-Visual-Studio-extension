// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace nanoFramework.Targeting.Tooling
{
    public sealed class NanoDevicesConfiguration
    {
        #region Fields
        private string? _virtualDeviceSerialPort;
        private readonly List<string> _reservedSerialPorts = [];
        private readonly Dictionary<string, List<string>> _deviceTypeTargets = [];
        private readonly List<string> _deviceTypes = [];
        private readonly List<string> _platforms = [];
        #endregion

        #region Properties
        /// <summary>
        /// The path to a local version of the <c>nano.exe</c> tool that should be used to
        /// run a Virtual nanoDevice. If this is <c></c>, the 
        /// </summary>
        public string? PathToLocalNanoCLR
        {
            get; private set;
        }

        /// <summary>
        /// The path to a directory that contains a file <c>nanoFramework.nanoCLR.dll</c>. The latter
        /// implements an alternative Virtual nanoDevice runtime. If the value is <c>null</c>, the runtime
        /// embedded in the <c>nanoclr.exe</c> file (<see cref="PathToLocalNanoCLR"/>) is used.
        /// </summary>
        public string? PathToLocalCLRInstanceDirectory
        {
            get; private set;
        }

        /// <summary>
        /// The serial port to use for a Virtual nanoDevice where applications can be deployed to.
        /// </summary>
        public string VirtualDeviceSerialPort
        {
            get => _virtualDeviceSerialPort ?? "COM30";
            private set => _virtualDeviceSerialPort = value;
        }

        /// <summary>
        /// The serial ports reserved for use by, e.g., a Virtual nanoDevice. These ports should be
        /// excluded in the discovery of real hardware nanoDevices. The <see cref="VirtualDeviceSerialPort"/>
        /// is not a member of this list.
        /// </summary>
        public IReadOnlyList<string> ReservedSerialPorts
            => _reservedSerialPorts;

        /// <summary>
        /// The path to the firmware archive directory.
        /// </summary>
        public string? FirmwareArchivePath
        {
            get; private set;
        }

        /// <summary>
        /// A collection of named device types a project can be deployed to.
        /// </summary>
        public ICollection<string> DeviceTypeNames
            => _deviceTypeTargets.Keys;

        /// <summary>
        /// The list of firmware/target names that are used for a named device type.
        /// </summary>
        /// <param name="name">Name of the device type; must be one of the <see cref="DeviceTypeNames"/>.</param>
        /// <returns></returns>
        public IReadOnlyList<string> DeviceTypeTargets(string name)
        {
            _deviceTypeTargets.TryGetValue(name, out List<string>? result);
            return result ?? [];
        }

        /// <summary>
        /// A list of device types the project is designed to be deployed to. The names are a subset
        /// of the <see cref="DeviceTypeNames"/>.
        /// </summary>
        public IReadOnlyList<string> DeviceTypes
            => _deviceTypes;

        /// <summary>
        /// A list of platforms the project is designed to be deployed to. This is shorthand to select all
        /// named devices in <see cref="DeviceTypeNames"/> that match the specified platform.
        /// </summary>
        public IReadOnlyList<string> Platforms
            => _platforms;
        #endregion
    }
}
