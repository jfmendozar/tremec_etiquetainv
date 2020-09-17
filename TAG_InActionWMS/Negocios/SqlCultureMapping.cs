using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TREMEC_EtiquetaInventario_WS.Negocios
{
    public class SqlCultureMapping
    {
        public string FullName { get; set; }
        public string Alias { get; set; }
        public string LCID { get; set; }
        public string specificulture { get; set; }

        public List<SqlCultureMapping> getSqlMappings()
        {

            List<SqlCultureMapping> obj = new List<SqlCultureMapping>()
        {
            new SqlCultureMapping {FullName="us_english", Alias="English", LCID="1033", specificulture="en-US"},
            new SqlCultureMapping {FullName="Deutsch", Alias="German", LCID="1031", specificulture="de-DE"},
            new SqlCultureMapping {FullName="Français", Alias="French", LCID="1036", specificulture="fr-FR"},
            new SqlCultureMapping {FullName="日本語", Alias="Japanese", LCID="1041", specificulture="ja-JP"},
            new SqlCultureMapping {FullName="Dansk", Alias="Danish", LCID="1030", specificulture="da-DK"},
            new SqlCultureMapping {FullName="Español", Alias="Spanish", LCID="3082", specificulture="es-MX"},
            new SqlCultureMapping {FullName="Italiano", Alias="Italian", LCID="1040", specificulture="it-IT"},
            new SqlCultureMapping {FullName="Nederlands", Alias="Dutch", LCID="1043", specificulture="nl-NL"},
            new SqlCultureMapping {FullName="Norsk", Alias="Norwegian", LCID="2068", specificulture="nn-NO"},
            new SqlCultureMapping {FullName="Português", Alias="Portuguese", LCID="2070", specificulture="pt-PT"},
            new SqlCultureMapping {FullName="Suomi", Alias="Finnish", LCID="1035", specificulture="fi"},
            new SqlCultureMapping {FullName="Svenska", Alias="Swedish", LCID="1053", specificulture="sv-SE"},
            new SqlCultureMapping {FullName="čeština", Alias="Czech", LCID="1029", specificulture="Cs-CZ"},
            new SqlCultureMapping {FullName="magyar", Alias="Hungarian", LCID="1038", specificulture="Hu-HU"},
            new SqlCultureMapping {FullName="polski", Alias="Polish", LCID="1045", specificulture="Pl-PL"},
            new SqlCultureMapping {FullName="română", Alias="Romanian", LCID="1048", specificulture="Ro-RO"},
            new SqlCultureMapping {FullName="hrvatski", Alias="Croatian", LCID="1050", specificulture="hr-HR"},
            new SqlCultureMapping {FullName="slovenčina", Alias="Slovak", LCID="1051", specificulture="Sk-SK"},
            new SqlCultureMapping {FullName="slovenski", Alias="Slovenian", LCID="1060", specificulture="Sl-SI"},
            new SqlCultureMapping {FullName="ελληνικά", Alias="Greek", LCID="1032", specificulture="El-GR"},
            new SqlCultureMapping {FullName="български", Alias="Bulgarian", LCID="1026", specificulture="bg-BG"},
            new SqlCultureMapping {FullName="русский", Alias="Russian", LCID="1049", specificulture="Ru-RU"},
            new SqlCultureMapping {FullName="Türkçe", Alias="Turkish", LCID="1055", specificulture="Tr-TR"},
            new SqlCultureMapping {FullName="British", Alias="British English", LCID="2057", specificulture="en-GB"},
            new SqlCultureMapping {FullName="eesti", Alias="Estonian", LCID="1061", specificulture="Et-EE"},
            new SqlCultureMapping {FullName="latviešu", Alias="Latvian", LCID="1062", specificulture="lv-LV"},
            new SqlCultureMapping {FullName="lietuvių", Alias="Lithuanian", LCID="1063", specificulture="lt-LT"},
            new SqlCultureMapping {FullName="Português (Brasil)", Alias="Brazilian", LCID="1046", specificulture="pt-BR"},
            new SqlCultureMapping {FullName="繁體中文", Alias="Traditional Chinese", LCID="1028", specificulture="zh-TW"},
            new SqlCultureMapping {FullName="한국어", Alias="Korean", LCID="1042", specificulture="Ko-KR"},
            new SqlCultureMapping {FullName="简体中文", Alias="Simplified Chinese", LCID="2052", specificulture="zh-CN"},
            new SqlCultureMapping {FullName="Arabic", Alias="Arabic", LCID="1025", specificulture="ar-SA"},
            new SqlCultureMapping {FullName="ไทย", Alias="Thai", LCID="1054", specificulture="Th-TH"}
        };

            return obj;
        }
    }
}