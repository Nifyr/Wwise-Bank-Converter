using Newtonsoft.Json;

namespace WwiseBankConverter
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            OpenFileDialog ofd = new()
            {
                Filter = "Wwise Bank(*.bnk;*.json)|*.bnk;*.json",
                RestoreDirectory = true
            };

            if (ofd.ShowDialog() != DialogResult.OK) return;

            string outputDir = Environment.CurrentDirectory + "\\Output";

            if (Path.GetExtension(ofd.FileName) == ".bnk")
            {
                try
                {
                    string destFileName = outputDir + "\\" + Path.GetFileNameWithoutExtension(ofd.FileName) + ".json";
                    Wwise.WwiseData wd = new(File.ReadAllBytes(ofd.FileName));
                    string json = JsonConvert.SerializeObject(wd.banks[0], Formatting.Indented);
                    Directory.CreateDirectory(outputDir);
                    File.WriteAllText(destFileName, json);
                    MessageBox.Show("Json placed in Output folder.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (Path.GetExtension(ofd.FileName) == ".json")
            {
                try
                {
                    string destFileName = outputDir + "\\" + Path.GetFileNameWithoutExtension(ofd.FileName) + ".bnk";
                    string json = File.ReadAllText(ofd.FileName);
                    Wwise.WwiseData wd = new()
                    {
                        banks = new()
                    };
                    Wwise.Bank? b = JsonConvert.DeserializeObject<Wwise.Bank>(json);
                    if (b == null)
                        throw new ArgumentException();
                    wd.banks.Add(b);
                    Directory.CreateDirectory(outputDir);
                    FileStream fs = File.OpenWrite(destFileName);
                    fs.Write(wd.GetBytes());
                    MessageBox.Show("Wwise bank placed in Output folder.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}