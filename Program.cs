using System.Text;
namespace CAS
{
    internal class Program {
        static void Usage()
        {
            Console.WriteLine($"Usage: simulator [options] -mc <machine_code_file>\n");
            Console.WriteLine($"Options:");
            Console.WriteLine($"  --cpu-type <type>     Type of CPU to simulate");
            Console.WriteLine($"                        Available: singlecycle (default)");
            Console.WriteLine($"  -mc <file>            Path to input machine code file (required)");
            Console.WriteLine($"  -dm <file>            Path to input data memory file (optional)");
            Console.WriteLine($"  -o <file>             Path to output file for register and data memory states (default: ./a.txt)");
            Console.WriteLine($"  --im-size <size>      Instruction memory total size in bytes (default: 1024)");
            Console.WriteLine($"  --dm-size <size>      Data memory total size in bytes (default: 1024)");
            Console.WriteLine();
            Console.WriteLine($"Example:");
            Console.WriteLine($"  {Environment.ProcessPath} --cpu-type singlecycle -mc program.mc -dm data.dm -o output.txt --im-size 2048 --dm-size 2048");
        }
        static void Main(string[] args) 
        {
            LibCPU.CPU_type? cpu_type = LibCPU.CPU_type.SingleCycle;
            string? mc_filepath = null;
            string? dm_filepath = null;
            string? output_filepath = "./a.txt";
            uint? IM_SIZE = 1024;
            uint? DM_SIZE = 1024;

            while (args.Length > 0)
            {
                Shartilities.ShiftArgs(ref args, out string arg);
                if (arg == "--cpu-type")
                {
                    if (!Shartilities.ShiftArgs(ref args, out string temp_cpu_type))
                        Shartilities.Log(Shartilities.LogType.ERROR, $"Missing argument \n", 1);
                    if (temp_cpu_type == "singlecycle")
                    {
                        cpu_type = LibCPU.CPU_type.SingleCycle;
                    }
                    else
                    {
                        Shartilities.Log(Shartilities.LogType.ERROR, $"invalid cpu type {temp_cpu_type}\n", 1);
                    }
                }
                else if (arg == "-mc")
                {
                    if (!Shartilities.ShiftArgs(ref args, out string temp_mc_filepath))
                        Shartilities.Log(Shartilities.LogType.ERROR, $"Missing argument \n", 1);
                    mc_filepath = temp_mc_filepath;
                }
                else if (arg == "-dm")
                {
                    if (!Shartilities.ShiftArgs(ref args, out string temp_dm_filepath))
                        Shartilities.Log(Shartilities.LogType.ERROR, $"Missing argument \n", 1);
                    dm_filepath = temp_dm_filepath;
                }
                else if (arg == "-o")
                {
                    if (!Shartilities.ShiftArgs(ref args, out string temp_output_filepath))
                        Shartilities.Log(Shartilities.LogType.ERROR, $"Missing argument \n", 1);
                    output_filepath = temp_output_filepath;
                }
                else if (arg == "--im-size")
                {
                    if (!Shartilities.ShiftArgs(ref args, out string temp_IM_SIZE))
                        Shartilities.Log(Shartilities.LogType.ERROR, $"Missing argument \n", 1);
                    if (uint.TryParse(temp_IM_SIZE, out uint temp_size))
                    {
                        IM_SIZE = temp_size;
                    }
                    else
                    {
                        Shartilities.Log(Shartilities.LogType.ERROR, $"could not parse instruction memory size {temp_IM_SIZE}\n", 1);
                        return;
                    }
                }
                else if (arg == "--dm-size")
                {
                    if (!Shartilities.ShiftArgs(ref args, out string temp_DM_SIZE))
                        Shartilities.Log(Shartilities.LogType.ERROR, $"Missing argument \n", 1);
                    if (uint.TryParse(temp_DM_SIZE, out uint temp_size))
                    {
                        DM_SIZE = temp_size;
                    }
                    else
                    {
                        Shartilities.Log(Shartilities.LogType.ERROR, $"could not parse data memory size {temp_DM_SIZE}\n", 1);
                        return;
                    }
                }
            }
            if (mc_filepath == null)
            {
                Shartilities.Log(Shartilities.LogType.ERROR, $"machine code file path was not provided\n", 1);
                Usage();
                return;
            }

            List<string> mcs = [.. File.ReadAllLines(mc_filepath)];

            List<string> data_mem_init = [];
            if (dm_filepath != null)
                data_mem_init = [.. File.ReadAllLines(dm_filepath)];

            if (cpu_type.Value == LibCPU.CPU_type.SingleCycle) {
                StringBuilder sb = new();
                (int cycles, List<int> regs, List<string> DM) = LibCPU.SingleCycle.Run(mcs, data_mem_init, IM_SIZE.Value, DM_SIZE.Value);
                sb.Append(LibCPU.RISCV.get_regs(regs));
                sb.Append(LibCPU.RISCV.get_DM(DM));
                sb.Append($"Number of cycles consumed : {cycles,10}\n");
                File.WriteAllText(output_filepath, sb.ToString());
            }
            else
            {
                Shartilities.Log(Shartilities.LogType.ERROR, $"simulating on {cpu_type} is unsupported for now\n", 1);
            }
        }
    }
}
