using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Input.Inking;

namespace ReactiveUIApp.Uwp.Extensions;

public static class InkStrokeContainerExtentions
{
    extension(InkStrokeContainer inkStrokeContainer)
    {
        public async Task<byte[]> SaveToBytesAsync(InkPersistenceFormat inkPersistenceFormat)
        {
            using var memoryStream = new InMemoryRandomAccessStream();

            await inkStrokeContainer.SaveAsync(memoryStream, inkPersistenceFormat);

            memoryStream.Seek(0);

            var buffer = new byte[memoryStream.Size];
            await memoryStream.ReadAsync(buffer.AsBuffer(), (uint)memoryStream.Size, InputStreamOptions.None);

            return buffer;
        }

        public async Task LoadFromBytesAsync(byte[] data)
        {
            using var memoryStream = new InMemoryRandomAccessStream();
            await memoryStream.WriteAsync(data.AsBuffer());

            memoryStream.Seek(0);

            await inkStrokeContainer.LoadAsync(memoryStream);
        }

        public int GetHashCodeOfStrokes()
        {
            var strokes = inkStrokeContainer.GetStrokes();

            if (strokes.Count >= 2)
            {
                return strokes
                    .Select(s => s.GetHashCode())
                    .Aggregate((current, hash) => HashCode.Combine(current, hash));
            }
            else if (strokes.Count == 1)
            {
                return strokes[0].GetHashCode();
            }
            else
            {
                return 0;
            }
        }
    }
}
