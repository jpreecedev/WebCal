/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package smartcardreader;

public enum TachographBrand
{
  NA(0, "No Information available", false), 

  ACTIA(16, "ACTIA", true), 

  GMBH(64, "Giesecke & Devrient Gmbh", false), 

  GEM_PLUS(65, "GEM plus", false), 

  INTELLIC(80, "EFAS", true), 

  OSCARD(128, "OSCARD", false), 

  SETEC(160, "SETEC", false), 

  SIEMENS(161, "SIEMENS VDO", true), 

  STONERIDGE(162, "STONERIDGE", false), 

  TACHOCONTROL(170, "TACHOCONTROL", false);

  private int value;
  private String name;
  private boolean visible;

  private TachographBrand(int paramInt, String paramString, boolean paramBoolean) { this.value = paramInt;
    this.name = paramString;
    this.visible = paramBoolean;
  }

  public String getName()
  {
    return this.name;
  }

  public boolean isVisible()
  {
    return this.visible;
  }

  public static TachographBrand getManufacturer(int paramInt)
  {
    for (TachographBrand localTachographBrand : values()) {
      if (localTachographBrand.value == paramInt) {
        return localTachographBrand;
      }
    }
    return null;
  }

  public static TachographBrand getManufacturer(String paramString)
  {
    for (TachographBrand localTachographBrand : values()) {
      if (localTachographBrand.name.equals(paramString)) {
        return localTachographBrand;
      }
    }
    return null;
  }
}