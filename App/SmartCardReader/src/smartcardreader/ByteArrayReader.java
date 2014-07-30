package smartcardreader;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;

public class ByteArrayReader extends ByteArrayInputStream
{
  public ByteArrayReader(byte[] paramArrayOfByte)
  {
    super(paramArrayOfByte);
  }

  public byte[] read(int paramInt)
  {
    ByteArrayOutputStream localByteArrayOutputStream = new ByteArrayOutputStream();

    for (int i = 0; i < paramInt; i++) {
      int j = super.read();

      if (j == -1) {
        throw new IllegalStateException("Premature end of stream");
      }

      localByteArrayOutputStream.write(j);
    }

    return localByteArrayOutputStream.toByteArray();
  }

  public void discard(int paramInt)
  {
    for (int i = 0; i < paramInt; i++) {
      int j = super.read();

      if (j == -1)
        throw new IllegalStateException("Premature end of stream");
    }
  }

  public void discard()
  {
    discard(1);
  }
}
