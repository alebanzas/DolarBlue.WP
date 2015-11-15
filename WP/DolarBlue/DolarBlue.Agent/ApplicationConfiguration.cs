using System;
using System.Device.Location;

namespace DolarBlueAgent
{
    public class ApplicationConfiguration
    {
        public ApplicationConfiguration()
        {
            Ubicacion = SetUbicacionDefault();
        }

        public ApplicationConfiguration(string name, string version)
        {
            Name = name;
            Version = version;
            Ubicacion = SetUbicacionDefault();
        }

        private static GeoPosition<GeoCoordinate> SetUbicacionDefault()
        {
            //obelisco de buenos aires
            return new GeoPosition<GeoCoordinate>(DateTimeOffset.UtcNow, new GeoCoordinate(-34.603722, -58.381594));
        }

        public void SetInitialConfiguration(string name, string version)
        {
            OpenCount++;
            Name = name;
            Version = version;

            if (IsInitialized) return;

            OpenCount = 1;
            Ubicacion = SetUbicacionDefault();
            InstallationId = Guid.NewGuid();
            IsLocationEnabledByPhone = true;
            IsLocationEnabledByAppConfig = true;
            IsInitialized = true;

            Config.Set(this);
        }
        
        public bool IsInitialized { get; set; }

        public bool IsLocationEnabled { get { return IsLocationEnabledByPhone && IsLocationEnabledByAppConfig; } }

        public bool IsLocationEnabledByPhone { get; set; }

        public bool IsLocationEnabledByAppConfig { get; set; }

        public Guid InstallationId { get; set; }

        private GeoPosition<GeoCoordinate> _ubicacion;
        public GeoPosition<GeoCoordinate> Ubicacion
        {
            get { return _ubicacion ?? (_ubicacion = new GeoPosition<GeoCoordinate>()); }
            set { _ubicacion = value; }
        }

        public double MinDiffGeography = 0.0001;

        private string _version;
        public string Version
        {
            private set
            {
                var v = value;
#if DEBUG
                v += "d";
#endif
                _version = v;
            }

            get { return _version; }
        }

        public string Name { get; private set; }

        public int OpenCount { get; set; }

        public string MobFoxID = "336b241302471376ed5709debc76bac3";

        public bool MobFoxInTestMode
        {
            get
            {
#if DEBUG
                return true;
#endif
                return false;
            }
        }

        public bool IsRated { get; set; }
    }
}