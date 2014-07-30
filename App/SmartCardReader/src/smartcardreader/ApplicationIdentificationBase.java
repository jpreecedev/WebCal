/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package smartcardreader;

import java.io.IOException;

public class ApplicationIdentificationBase
{
  private static final long serialVersionUID = 1L;
  protected CardType cardType;
  protected int cardStructureVersion;
  protected int numberOfEventsperType;
  protected int numberOfFaultsPerType;
  protected int activityStructureLength;
  protected int numberOfCardVehicleRecords;
  protected int numberOfPlaceRecords;

  public static CardType checkCardType(byte[] paramArrayOfByte)
  {
    try
    {
      ByteArrayReader localByteArrayReader = new ByteArrayReader(paramArrayOfByte); Object localObject1 = null;
      try { return CardType.findType(localByteArrayReader.read()); }
      catch (Throwable localThrowable1)
      {
        localObject1 = localThrowable1; throw localThrowable1;
      } finally {
        if (localByteArrayReader != null) if (localObject1 != null) try { localByteArrayReader.close(); } catch (Throwable localThrowable3) {  } else localByteArrayReader.close();  
      } } catch (IOException localIOException) {  }

    return null;
  }

  public CardType getCardType()
  {
    return this.cardType;
  }

  public int getActivityStructureLength()
  {
    return this.activityStructureLength;
  }

  public int getNumberOfCardVehicleRecords()
  {
    return this.numberOfCardVehicleRecords;
  }

  public int getNumberOfPlaceRecords()
  {
    return this.numberOfPlaceRecords;
  }

  public int getNumberOfEventsPerType()
  {
    return this.numberOfEventsperType;
  }

  public int getNumberOfFaultsPerType()
  {
    return this.numberOfFaultsPerType;
  }

  public static enum CardType
  {
    RESERVED(0), 

    DRIVER(1), 

    WORKSHOP(2), 

    CONTROL(3), 

    COMPANY(4), 

    MANUFACTURING(5), 

    VEHICLE_UNIT(6), 

    MOTION_SENSOR(7), 

    OTHER(2147483647);

    private final int value;

    private CardType(int paramInt)
    {
      this.value = paramInt;
    }

    public static CardType findType(int paramInt)
    {
      for (CardType localCardType : values()) {
        if (localCardType.value == paramInt) {
          return localCardType;
        }
      }
      return OTHER;
    }
  }
}