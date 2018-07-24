using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Plot.ArcgisFrameWork.Utils
{
    public class LiecenseUtils
    {
        public static bool LoadBaseLicense(out string ErrorMessage)
        { 
            bool result = true;
            ErrorMessage = ""; 
             //检测ArcGIS授权
            if (!RuntimeManager.Bind(ProductCode.EngineOrDesktop))
            {
                ErrorMessage = "请确认ArcGIS的许可安装正确有效！";
                return false;
            }
            IAoInitialize aoInitialize = new AoInitialize();
            try
            {
                esriLicenseStatus licenseStatus = esriLicenseStatus.esriLicenseUnavailable;
                licenseStatus = aoInitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced);
                if (licenseStatus != esriLicenseStatus.esriLicenseCheckedOut)
                {
                    licenseStatus = aoInitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB);
                    if (licenseStatus != esriLicenseStatus.esriLicenseCheckedOut)
                    {
                        ErrorMessage = "获取ARCGIS授权失败";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            { 
                result = false;
                ErrorMessage = ex.Message;
            }
            return result;
        }

        public static bool LoadAdvanceLicense(bool isLoadSpatialAnalyst,bool isLoadCodeNetwork,out string ErrorMessage)
        {
            bool result = true;
            ErrorMessage = "";
            //检测ArcGIS授权
            if (!RuntimeManager.Bind(ProductCode.EngineOrDesktop))
            {
                ErrorMessage = "请确认ArcGIS的许可安装正确有效！";
                return false;
            }
            IAoInitialize aoInitialize = new AoInitialize();
            try
            {
                esriLicenseStatus licenseStatus = esriLicenseStatus.esriLicenseUnavailable;
                licenseStatus = aoInitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced);
                if (licenseStatus != esriLicenseStatus.esriLicenseCheckedOut)
                {
                    licenseStatus = aoInitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB);
                    if (licenseStatus != esriLicenseStatus.esriLicenseCheckedOut)
                    {
                        ErrorMessage = "获取ARCGIS授权失败";
                        return false;
                    }
                }
                // 迁出空间分析扩展模块
                if (isLoadSpatialAnalyst)
                {
                    if (aoInitialize.CheckOutExtension(esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst) != esriLicenseStatus.esriLicenseCheckedOut)
                    {
                        ErrorMessage = "获取空间分析扩展模块授权失败！";
                        return false;
                    }
                }
                // 迁出网络分析扩展模块
                if (isLoadCodeNetwork)
                {
                    if (aoInitialize.CheckOutExtension(esriLicenseExtensionCode.esriLicenseExtensionCodeNetwork) != esriLicenseStatus.esriLicenseCheckedOut)
                    {
                         ErrorMessage = "获取网络分析扩展模块授权失败！";
                         return false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                ErrorMessage = ex.Message;
            }
            return result;
        }
    }
}
