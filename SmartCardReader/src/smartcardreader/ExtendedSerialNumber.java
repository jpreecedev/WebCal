/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package smartcardreader;

import java.util.Arrays;

public class ExtendedSerialNumber
{
  private static final long serialVersionUID = 1L;
  private final long serialNumber;
  private final String monthYear;
  private final byte type;
  private TachographBrand manufacturer;

  public ExtendedSerialNumber(byte[] paramArrayOfByte)
  {
    assert (paramArrayOfByte.length == 8);
    this.serialNumber = Utilities.toLong(Arrays.copyOfRange(paramArrayOfByte, 0, 4));
    this.monthYear = Utilities.parseBCD(Arrays.copyOfRange(paramArrayOfByte, 4, 6));
    this.type = paramArrayOfByte[6];
    this.manufacturer = TachographBrand.getManufacturer(paramArrayOfByte[7] & 0xFF);
    if (this.manufacturer == null)
      this.manufacturer = TachographBrand.NA;
  }

  public long getSerialNumber()
  {
    return this.serialNumber;
  }

  public String getMontYear()
  {
    return this.monthYear;
  }

  public byte getType()
  {
    return this.type;
  }

  public TachographBrand getManufacturer()
  {
    return this.manufacturer;
  }

  @Override
  public String toString()
  {
    return "ExtendedSerialNumber [manufacturer=" + this.manufacturer + ", monthYear=" + this.monthYear + ", serialNumber=" + this.serialNumber + ", type=" + this.type + "]";
  }
}