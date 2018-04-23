using System;
using System.Collections.Generic;
using Microsoft.StylusInput;
using Microsoft.StylusInput.PluginData;

namespace Uno.Support.WinForms
{
    public class TouchEventAdapter
    {
        public event EventHandler<TouchEventArgs> TouchDown;
        public event EventHandler<TouchEventArgs> TouchMove;
        public event EventHandler<TouchEventArgs> TouchUp;

        public IStylusSyncPlugin StylusSyncPlugin 
        { 
            get; }

        public TouchEventAdapter(UnoGLControl form)
        {
            var graphics = form.CreateGraphics();
            StylusSyncPlugin = new StylusSyncPluginImpl(form, this, graphics.DpiX, graphics.DpiY);

            var stylus = new RealTimeStylus(form);
            stylus.MultiTouchEnabled = true;
            stylus.SyncPluginCollection.Add(StylusSyncPlugin);
            stylus.Enabled = true;
        }

        class StylusSyncPluginImpl : IStylusSyncPlugin
        {
            readonly UnoGLControl _form;
            readonly TouchEventAdapter _touch;
            readonly float _dpiX;
            readonly float _dpiY;

            public StylusSyncPluginImpl(UnoGLControl form, TouchEventAdapter touch, float dpiX, float dpiY)
            {
                _form = form;
                _touch = touch;
                _dpiX = dpiX;
                _dpiY = dpiY;
            }

            void IStylusSyncPlugin.RealTimeStylusEnabled(RealTimeStylus sender, RealTimeStylusEnabledData data) { }
            void IStylusSyncPlugin.RealTimeStylusDisabled(RealTimeStylus sender, RealTimeStylusDisabledData data) { }
            void IStylusSyncPlugin.StylusInRange(RealTimeStylus sender, StylusInRangeData data) { }
            void IStylusSyncPlugin.StylusOutOfRange(RealTimeStylus sender, StylusOutOfRangeData data) { }
            void IStylusSyncPlugin.StylusDown(RealTimeStylus sender, StylusDownData data)
            {
                if (_touch.TouchDown != null)
                    _form.BeginInvoke((Action)(() =>
                        {
                            foreach (var arg in GetEvents(data))
                                _touch.TouchDown(sender, arg);
                        }));
            }
            void IStylusSyncPlugin.StylusUp(RealTimeStylus sender, StylusUpData data)
            {
                if (_touch.TouchUp != null)
                    _form.BeginInvoke((Action)(() =>
                        {
                            foreach (var arg in GetEvents(data))
                                _touch.TouchUp(sender, arg);
                        }));
            }
            void IStylusSyncPlugin.StylusButtonDown(RealTimeStylus sender, StylusButtonDownData data) { }
            void IStylusSyncPlugin.StylusButtonUp(RealTimeStylus sender, StylusButtonUpData data) { }
            void IStylusSyncPlugin.InAirPackets(RealTimeStylus sender, InAirPacketsData data) { }
            void IStylusSyncPlugin.Packets(RealTimeStylus sender, PacketsData data)
            {
                if (_touch.TouchMove != null)
                    _form.BeginInvoke((Action)(() =>
                        {
                            foreach (var arg in GetEvents(data))
                                _touch.TouchMove(sender, arg);
                        }));
            }
            void IStylusSyncPlugin.SystemGesture(RealTimeStylus sender, SystemGestureData data) { }
            void IStylusSyncPlugin.TabletAdded(RealTimeStylus sender, TabletAddedData data) { }
            void IStylusSyncPlugin.TabletRemoved(RealTimeStylus sender, TabletRemovedData data) { }
            void IStylusSyncPlugin.CustomStylusDataAdded(RealTimeStylus sender, CustomStylusData data) { }
            void IStylusSyncPlugin.Error(RealTimeStylus sender, ErrorData data) { }

            DataInterestMask IStylusSyncPlugin.DataInterest => DataInterestMask.StylusDown | DataInterestMask.StylusUp | DataInterestMask.Packets;

            IEnumerable<TouchEventArgs> GetEvents(StylusDataBase data)
            {
                for (int i = 0; i < data.Count; i += data.PacketPropertyCount)
                {
                    float x = data[i];
                    float y = data[i + 1];
                    if (data.Stylus.Name != "Mouse") // TODO: hack
                        yield return new TouchEventArgs
                        {
                            Index = data.Stylus.Id,
                            X = Math.Floor(x * _dpiX / 2540.0F),
                            Y = Math.Floor(y * _dpiY / 2540.0F)
                        };
                }
            }
        }
    }
}