/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package smartcardreader;

public enum CalibrationPurpose
{
  RESERVED(0), 

  ACTIVATION(1), 

  FIRST_INSTALLATION(2), 

  INSTALLATION(3), 

  INSPECTION(4);

  private int value;

  private CalibrationPurpose(int paramInt) {
    this.value = paramInt;
  }

  public static CalibrationPurpose get(int paramInt)
  {
    for (CalibrationPurpose localCalibrationPurpose : values()) {
      if (localCalibrationPurpose.value == paramInt) {
        return localCalibrationPurpose;
      }
    }

    return null;
  }
}