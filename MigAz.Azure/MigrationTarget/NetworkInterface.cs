﻿using MigAz.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigAz.Azure.MigrationTarget
{
    public class NetworkInterface : IMigrationTarget
    {
        private AzureContext _AzureContext;
        private INetworkInterface _SourceNetworkInterface;
        private bool _EnableIPForwarding = false;
        private List<MigrationTarget.NetworkInterfaceIpConfiguration> _TargetNetworkInterfaceIpConfigurations = new List<MigrationTarget.NetworkInterfaceIpConfiguration>();
        private List<LoadBalancerRule> _LoadBalancerRules = new List<LoadBalancerRule>();
        private string _TargetName = String.Empty;

        private NetworkInterface() { }

        public NetworkInterface(AzureContext azureContext, Asm.NetworkInterface networkInterface, List<VirtualNetwork> virtualNetworks)
        {
            _AzureContext = azureContext;
            _SourceNetworkInterface = networkInterface;
            this.TargetName = networkInterface.Name;
            this.IsPrimary = networkInterface.IsPrimary;

            foreach (Asm.NetworkInterfaceIpConfiguration asmNetworkInterfaceIpConfiguration in networkInterface.NetworkInterfaceIpConfigurations)
            {
                Azure.MigrationTarget.NetworkInterfaceIpConfiguration migrationNetworkInterfaceIpConfiguration = new Azure.MigrationTarget.NetworkInterfaceIpConfiguration(_AzureContext, asmNetworkInterfaceIpConfiguration, virtualNetworks);
                this.TargetNetworkInterfaceIpConfigurations.Add(migrationNetworkInterfaceIpConfiguration);
            }
        }

        public NetworkInterface(AzureContext azureContext, Arm.NetworkInterface networkInterface, List<VirtualNetwork> virtualNetworks)
        {
            _AzureContext = azureContext;
            _SourceNetworkInterface = networkInterface;
            this.TargetName = networkInterface.Name;
            this.IsPrimary = networkInterface.IsPrimary;

            foreach (Arm.NetworkInterfaceIpConfiguration armNetworkInterfaceIpConfiguration in networkInterface.NetworkInterfaceIpConfigurations)
            {
                MigrationTarget.NetworkInterfaceIpConfiguration targetNetworkInterfaceIpConfiguration = new NetworkInterfaceIpConfiguration(azureContext, armNetworkInterfaceIpConfiguration, virtualNetworks);
                this.TargetNetworkInterfaceIpConfigurations.Add(targetNetworkInterfaceIpConfiguration);
            }
        }

        public List<MigrationTarget.NetworkInterfaceIpConfiguration> TargetNetworkInterfaceIpConfigurations
        {
            get { return _TargetNetworkInterfaceIpConfigurations; }
        }

        public bool EnableIPForwarding
        {
            get { return _EnableIPForwarding; }
            set { _EnableIPForwarding = value; }
        }

        public string TargetName
        {
            get { return _TargetName; }
            set { _TargetName = value.Trim().Replace(" ", String.Empty); }
        }

        public override string ToString()
        {
            return this.TargetName + _AzureContext.SettingsProvider.NetworkInterfaceCardSuffix;
        }

        public bool HasPublicIPs
        {
            get { return false; } // todo now asap
        }
        public bool IsPrimary { get; set; }

        public INetworkSecurityGroup NetworkSecurityGroup
        {
            get; set; // todo now asap
        }

        public string LoadBalancerName
        {
            get;set;
        }

        public List<LoadBalancerRule> LoadBalancerRules
        {
            get { return _LoadBalancerRules; } // todo now asap
        }

        public INetworkInterface SourceNetworkInterface
        {
            get { return _SourceNetworkInterface; }
        }

        public String SourceName
        {
            get
            {
                if (this.SourceNetworkInterface == null)
                    return String.Empty;
                else
                    return this.SourceNetworkInterface.ToString();
            }
        }
    }
}
