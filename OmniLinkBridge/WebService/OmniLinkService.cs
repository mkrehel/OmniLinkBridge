﻿using HAI_Shared;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace OmniLinkBridge.WebAPI
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class OmniLinkService : IOmniLinkService
    {
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Subscribe(SubscribeContract contract)
        {
            log.Debug("Subscribe");
            WebNotification.AddSubscription(contract.callback);
        }

        public List<NameContract> ListAreas()
        {
            log.Debug("ListAreas");

            List<NameContract> names = new List<NameContract>();
            for (ushort i = 1; i < WebServiceModule.OmniLink.Controller.Areas.Count; i++)
            {
                clsArea area = WebServiceModule.OmniLink.Controller.Areas[i];

                if (area.DefaultProperties == false)
                    names.Add(new NameContract() { id = i, name = area.Name });
            }
            return names;
        }

        public AreaContract GetArea(ushort id)
        {
            log.Debug("GetArea: " + id);

            WebOperationContext ctx = WebOperationContext.Current;
            ctx.OutgoingResponse.Headers.Add("type", "area");

            return WebServiceModule.OmniLink.Controller.Areas[id].ToContract();
        }

        public List<NameContract> ListZonesContact()
        {
            log.Debug("ListZonesContact");

            List<NameContract> names = new List<NameContract>();
            for (ushort i = 1; i < WebServiceModule.OmniLink.Controller.Zones.Count; i++)
            {
                clsZone zone = WebServiceModule.OmniLink.Controller.Zones[i];

                if ((zone.ZoneType == enuZoneType.EntryExit ||
                    zone.ZoneType == enuZoneType.X2EntryDelay ||
                    zone.ZoneType == enuZoneType.X4EntryDelay ||
                    zone.ZoneType == enuZoneType.Perimeter ||
                    zone.ZoneType == enuZoneType.Tamper ||
                    zone.ZoneType == enuZoneType.Auxiliary) && zone.DefaultProperties == false)
                    names.Add(new NameContract() { id = i, name = zone.Name });
            }
            return names;
        }

        public List<NameContract> ListZonesMotion()
        {
            log.Debug("ListZonesMotion");

            List<NameContract> names = new List<NameContract>();
            for (ushort i = 1; i < WebServiceModule.OmniLink.Controller.Zones.Count; i++)
            {
                clsZone zone = WebServiceModule.OmniLink.Controller.Zones[i];

                if ((zone.ZoneType == enuZoneType.AwayInt ||
                    zone.ZoneType == enuZoneType.NightInt) && zone.DefaultProperties == false)
                    names.Add(new NameContract() { id = i, name = zone.Name });
            }
            return names;
        }

        public List<NameContract> ListZonesWater()
        {
            log.Debug("ListZonesWater");

            List<NameContract> names = new List<NameContract>();
            for (ushort i = 1; i < WebServiceModule.OmniLink.Controller.Zones.Count; i++)
            {
                clsZone zone = WebServiceModule.OmniLink.Controller.Zones[i];

                if (zone.ZoneType == enuZoneType.Water && zone.DefaultProperties == false)
                    names.Add(new NameContract() { id = i, name = zone.Name });
            }
            return names;
        }

        public List<NameContract> ListZonesSmoke()
        {
            log.Debug("ListZonesSmoke");

            List<NameContract> names = new List<NameContract>();
            for (ushort i = 1; i < WebServiceModule.OmniLink.Controller.Zones.Count; i++)
            {
                clsZone zone = WebServiceModule.OmniLink.Controller.Zones[i];

                if (zone.ZoneType == enuZoneType.Fire && zone.DefaultProperties == false)
                    names.Add(new NameContract() { id = i, name = zone.Name });
            }
            return names;
        }

        public List<NameContract> ListZonesCO()
        {
            log.Debug("ListZonesCO");

            List<NameContract> names = new List<NameContract>();
            for (ushort i = 1; i < WebServiceModule.OmniLink.Controller.Zones.Count; i++)
            {
                clsZone zone = WebServiceModule.OmniLink.Controller.Zones[i];

                if (zone.ZoneType == enuZoneType.Gas && zone.DefaultProperties == false)
                    names.Add(new NameContract() { id = i, name = zone.Name });
            }
            return names;
        }

        public List<NameContract> ListZonesTemp()
        {
            log.Debug("ListZonesTemp");

            List<NameContract> names = new List<NameContract>();
            for (ushort i = 1; i < WebServiceModule.OmniLink.Controller.Zones.Count; i++)
            {
                clsZone zone = WebServiceModule.OmniLink.Controller.Zones[i];

                if (zone.IsTemperatureZone() && zone.DefaultProperties == false)
                    names.Add(new NameContract() { id = i, name = zone.Name });
            }
            return names;
        }

        public ZoneContract GetZone(ushort id)
        {
            log.Debug("GetZone: " + id);

            WebOperationContext ctx = WebOperationContext.Current;

            if (WebServiceModule.OmniLink.Controller.Zones[id].IsTemperatureZone())
            {
                ctx.OutgoingResponse.Headers.Add("type", "temp");
            }
            else
            {
                switch (WebServiceModule.OmniLink.Controller.Zones[id].ZoneType)
                {
                    case enuZoneType.EntryExit:
                    case enuZoneType.X2EntryDelay:
                    case enuZoneType.X4EntryDelay:
                    case enuZoneType.Perimeter:
                    case enuZoneType.Tamper:
                    case enuZoneType.Auxiliary:
                        ctx.OutgoingResponse.Headers.Add("type", "contact");
                        break;
                    case enuZoneType.AwayInt:
                    case enuZoneType.NightInt:
                        ctx.OutgoingResponse.Headers.Add("type", "motion");
                        break;
                    case enuZoneType.Water:
                        ctx.OutgoingResponse.Headers.Add("type", "water");
                        break;
                    case enuZoneType.Fire:
                        ctx.OutgoingResponse.Headers.Add("type", "smoke");
                        break;
                    case enuZoneType.Gas:
                        ctx.OutgoingResponse.Headers.Add("type", "co");
                        break;
                    default:
                        ctx.OutgoingResponse.Headers.Add("type", "unknown");
                        break;
                }
            }

            return WebServiceModule.OmniLink.Controller.Zones[id].ToContract();
        }

        public List<NameContract> ListUnits()
        {
            log.Debug("ListUnits");

            List<NameContract> names = new List<NameContract>();
            for (ushort i = 1; i < WebServiceModule.OmniLink.Controller.Units.Count; i++)
            {
                clsUnit unit = WebServiceModule.OmniLink.Controller.Units[i];

                if (unit.DefaultProperties == false)
                    names.Add(new NameContract() { id = i, name = unit.Name });
            }
            return names;
        }

        public UnitContract GetUnit(ushort id)
        {
            log.Debug("GetUnit: " + id);

            WebOperationContext ctx = WebOperationContext.Current;
            ctx.OutgoingResponse.Headers.Add("type", "unit");

            return WebServiceModule.OmniLink.Controller.Units[id].ToContract();
        }

        public void SetUnit(CommandContract unit)
        {
            log.Debug("SetUnit: " + unit.id + " to " + unit.value + "%");

            if (unit.value == 0)
                WebServiceModule.OmniLink.Controller.SendCommand(enuUnitCommand.Off, 0, unit.id);
            else if (unit.value == 100)
                WebServiceModule.OmniLink.Controller.SendCommand(enuUnitCommand.On, 0, unit.id);
            else
                WebServiceModule.OmniLink.Controller.SendCommand(enuUnitCommand.Level, BitConverter.GetBytes(unit.value)[0], unit.id);
        }


        public void SetUnitKeypadPress(CommandContract unit)
        {
            log.Debug("SetUnitKeypadPress: " + unit.id + " to " + unit.value + " button");
            WebServiceModule.OmniLink.Controller.SendCommand(enuUnitCommand.LutronHomeWorksKeypadButtonPress, BitConverter.GetBytes(unit.value)[0], unit.id);
        }

        public List<NameContract> ListThermostats()
        {
            log.Debug("ListThermostats");

            List<NameContract> names = new List<NameContract>();
            for (ushort i = 1; i < WebServiceModule.OmniLink.Controller.Thermostats.Count; i++)
            {
                clsThermostat unit = WebServiceModule.OmniLink.Controller.Thermostats[i];

                if (unit.DefaultProperties == false)
                    names.Add(new NameContract() { id = i, name = unit.Name });
            }
            return names;
        }

        public ThermostatContract GetThermostat(ushort id)
        {
            log.Debug("GetThermostat: " + id);

            WebOperationContext ctx = WebOperationContext.Current;
            ctx.OutgoingResponse.Headers.Add("type", "thermostat");

            return WebServiceModule.OmniLink.Controller.Thermostats[id].ToContract();
        }

        public void SetThermostatCoolSetpoint(CommandContract unit)
        {
            int temp = ((double)unit.value).ToCelsius().ToOmniTemp();
            log.Debug("SetThermostatCoolSetpoint: " + unit.id + " to " + unit.value + "F (" + temp + ")");
            WebServiceModule.OmniLink.Controller.SendCommand(enuUnitCommand.SetHighSetPt, BitConverter.GetBytes(temp)[0], unit.id);
        }

        public void SetThermostatHeatSetpoint(CommandContract unit)
        {
            int temp = ((double)unit.value).ToCelsius().ToOmniTemp();
            log.Debug("SetThermostatCoolSetpoint: " + unit.id + " to " + unit.value + "F (" + temp + ")");
            WebServiceModule.OmniLink.Controller.SendCommand(enuUnitCommand.SetLowSetPt, BitConverter.GetBytes(temp)[0], unit.id);
        }

        public void SetThermostatMode(CommandContract unit)
        {
            log.Debug("SetThermostatMode: " + unit.id + " to " + unit.value);
            WebServiceModule.OmniLink.Controller.SendCommand(enuUnitCommand.Mode, BitConverter.GetBytes(unit.value)[0], unit.id);
        }

        public void SetThermostatFanMode(CommandContract unit)
        {
            log.Debug("SetThermostatFanMode: " + unit.id + " to " + unit.value);
            WebServiceModule.OmniLink.Controller.SendCommand(enuUnitCommand.Fan, BitConverter.GetBytes(unit.value)[0], unit.id);
        }

        public void SetThermostatHold(CommandContract unit)
        {
            log.Debug("SetThermostatHold: " + unit.id + " to " + unit.value);
            WebServiceModule.OmniLink.Controller.SendCommand(enuUnitCommand.Hold, BitConverter.GetBytes(unit.value)[0], unit.id);
        }

        public List<NameContract> ListButtons()
        {
            log.Debug("ListButtons");

            List<NameContract> names = new List<NameContract>();
            for (ushort i = 1; i < WebServiceModule.OmniLink.Controller.Buttons.Count; i++)
            {
                clsButton unit = WebServiceModule.OmniLink.Controller.Buttons[i];

                if (unit.DefaultProperties == false)
                    names.Add(new NameContract() { id = i, name = unit.Name });
            }
            return names;
        }

        public void PushButton(CommandContract unit)
        {
            log.Debug("PushButton: " + unit.id);
            WebServiceModule.OmniLink.Controller.SendCommand(enuUnitCommand.Button, 0, unit.id);
        }
    }
}