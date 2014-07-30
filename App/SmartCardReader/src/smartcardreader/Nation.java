/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package smartcardreader;

public enum Nation
{
  NA(0, " "), 

  AUSTRIA(1, "A"), 

  ALBANIA(2, "AL"), 

  ANDORRA(3, "AND"), 

  ARMENIA(4, "ARM"), 

  AZERBAIJAN(5, "AZ"), 

  BELGIUM(6, "B"), 

  BULGARIA(7, "G"), 

  BOSNIA_AND_HERZEGOVINA(8, "BIH"), 

  BELARUS(9, "BY"), 

  SWITZERLAND(10, "CH"), 

  CYPRUS(11, "CY"), 

  CZECH_REPUBLIC(12, "CZ"), 

  GERMANY(13, "D"), 

  DENMARK(14, "DK"), 

  SPAIN(15, "E"), 

  ESTONIA(16, "EST"), 

  FRANCE(17, "F"), 

  FINLAND(18, "FIN"), 

  LIECHTENSTEIN(19, "FL"), 

  FAEROE_ISLANDS(20, "FR"), 

  UNITED_KINGDOM(21, "UK"), 

  GEORGIA(22, "GE"), 

  GREECE(23, "GR"), 

  HUNGARY(24, "H"), 

  CROATIA(25, "HR"), 

  ITALY(26, "I"), 

  IRELAND(27, "IRL"), 

  ICELAND(28, "IS"), 

  KAZAKHSTAN(29, "KZ"), 

  LUXEMBOURG(30, "L"), 

  LITHUANIA(31, "LT"), 

  LATVIA(32, "LV"), 

  MALTA(33, "M"), 

  MONACO(34, "MC"), 

  REPUBLIC_OF_MOLDOVA(35, "MD"), 

  MACEDONIA(36, "MK"), 

  NORWAY(37, "N"), 

  THE_NETHERLANDS(38, "NL"), 

  PORTUGAL(39, "P"), 

  POLAND(40, "PL"), 

  ROMANIA(41, "RO"), 

  SAN_MARINO(42, "RSM"), 

  RUSSIAN_FEDERATION(43, "RUS"), 

  SWEDEN(44, "S"), 

  SLOVAKIA(45, "SK"), 

  SLOVENIA(46, "SLO"), 

  TURKMENISTAN(47, "TM"), 

  TURKEY(48, "TR"), 

  UKRAINE(49, "UA"), 

  VATICAN_CITY(50, "V"), 

  YUGOSLAVIA(51, "YU"), 

  EUROPEAN_COMMUNITY(253, "EC"), 

  REST_OF_EUROPE(254, "EUR"), 

  REST_OF_THE_WORLD(255, "WLD");

  private final int value;
  private final String code;

  private Nation(int paramInt, String paramString) {
    this.value = paramInt;
    this.code = paramString;
  }

  public String getCode()
  {
    return this.code;
  }

  public static Nation getNation(int paramInt)
  {
    for (Nation localNation : values()) {
      if (localNation.value == paramInt) {
        return localNation;
      }
    }
    return null;
  }
}