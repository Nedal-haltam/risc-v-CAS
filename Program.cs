using System.Text;
namespace main {
    internal class Program {
        static List<string> mcs = [];
        static List<string> data_mem_init = [];
        static string output_filepath = "";
        static int IM_SIZE;
        static int DM_SIZE;
        static LibCPU.CPU_type cpu_type = LibCPU.CPU_type.SingleCycle;
        static void HandleCommand(string[] args) {

            Shartilities.ShiftArgs(ref args, out string sim);
            if (sim.ToLower() == "sim") {
                Shartilities.ShiftArgs(ref args, out string cputype);
                if (cputype == "singlecycle")   { cpu_type = LibCPU.CPU_type.SingleCycle; }
                else if (cputype == "pipeline") { cpu_type = LibCPU.CPU_type.PipeLined; }
                else
                { 
                    Shartilities.Assert(false, $"invalid cpu type {cputype}"); 
                }
                Shartilities.ShiftArgs(ref args, out string mc_filepath);
                Shartilities.ShiftArgs(ref args, out string dm_filepath);
                Shartilities.ShiftArgs(ref args, out string outputfilepath);
                Shartilities.ShiftArgs(ref args, out string IM_SIZEtext);
                Shartilities.ShiftArgs(ref args, out string DM_SIZEtext);

                int.TryParse(IM_SIZEtext, out int IMSIZE);
                IM_SIZE = IMSIZE;
                int.TryParse(DM_SIZEtext, out int DMSIZE);
                DM_SIZE = DMSIZE;
                output_filepath = outputfilepath;

                mcs = File.ReadAllLines(mc_filepath).ToList();
                data_mem_init = File.ReadAllLines(dm_filepath).ToList();
            }
            else 
            { 
                Shartilities.Assert(false, $"Invalid argument {sim}"); 
            }
        }


        static void Main(string[] args) {
            if (args.Length != 7)
                Shartilities.Log(Shartilities.LogType.ERROR, $"missing arguments\n", 1);
            HandleCommand(args);
            
            StringBuilder sb = new StringBuilder();
            if (cpu_type == LibCPU.CPU_type.SingleCycle) {
                LibCPU.RISCV.IM_SIZE = IM_SIZE;
                LibCPU.RISCV.DM_SIZE = DM_SIZE;
                LibCPU.SingleCycle cpu = new(mcs, data_mem_init);
                (int cycles, LibCPU.RISCV.Exceptions excep) = cpu.Run();
                sb.Append(LibCPU.RISCV.get_regs(cpu.regs));
                sb.Append(LibCPU.RISCV.get_DM(cpu.DM));
                //sb.Append($"Exception Type : {excep.ToString()}");
                sb.Append($"Number of cycles consumed : {cycles,10}\n");
            }
            else if (cpu_type == LibCPU.CPU_type.PipeLined) {
                LibCPU.RISCV.IM_SIZE = IM_SIZE;
                LibCPU.RISCV.DM_SIZE = DM_SIZE;
                LibCPU.CPU6STAGE cpu = new(mcs, data_mem_init);
                (int cycles, LibCPU.RISCV.Exceptions excep) = cpu.Run();
                sb.Append(LibCPU.RISCV.get_regs(cpu.regs));
                sb.Append(LibCPU.RISCV.get_DM(cpu.DM));
                //sb.Append($"Exception Type : {excep.ToString()}");
                sb.Append($"Number of cycles consumed : {cycles,10}\n");
            }
            else if (cpu_type == LibCPU.CPU_type.OOO) {
                LibCPU.RISCV.IM_SIZE = IM_SIZE;
                LibCPU.RISCV.DM_SIZE = DM_SIZE;
                LibCPU.OOO cpu = new(mcs, data_mem_init);
                (int cycles, LibCPU.RISCV.Exceptions excep) = cpu.Run();
                sb.Append(LibCPU.RISCV.get_regs(cpu.regs));
                sb.Append(LibCPU.RISCV.get_DM(cpu.DM));
                //sb.Append($"Exception Type : {excep.ToString()}");
                sb.Append($"Number of cycles consumed : {cycles,10}\n");
            }
            File.WriteAllText(output_filepath, sb.ToString());
        }
    }
}
