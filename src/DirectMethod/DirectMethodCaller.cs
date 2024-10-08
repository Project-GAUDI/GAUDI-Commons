using System;
using System.Reflection;
using System.Threading.Tasks;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// Direct Method Calling class
    /// </summary>
    public class DirectMethodCaller
    {
        /// <summary>
        /// DirectMethod 呼び出しクラス
        /// </summary>
        /// <param name="methodRequest"></param>
        /// <returns></returns>
        public static async Task<DirectMethodResponse> Run(DirectMethodRequest methodRequest)
        {
            DirectMethodResponse resp = new DirectMethodResponse(-1);

            IDirectMethodRunner runner = null;
            bool status = false;

            try
            {
                runner = GetRunner(methodRequest.MethodName);
                if (null != runner)
                {
                    status = true;
                }
                else
                {
                    status = false;
                }

                if (status)
                {
                    status = await runner.ParseRequest(methodRequest.RequestJson);
                }

                if (status)
                {
                    status = await runner.Run();
                }

                resp = runner.GetResult();

            }
            catch (Exception ex)
            {
                status = false;
                resp.Status = -1;
                resp.Results.Add("Error", $"Direct Method({methodRequest.MethodName}) execution failed. ({ex})");
            }

            return resp;
        }

        protected static IDirectMethodRunner GetRunner(string methodName)
        {
            IDirectMethodRunner retRunner = null;
            try
            {
                if (false == String.IsNullOrEmpty(methodName))
                {
                    string SearchMethodName = "DirectMethod_" + methodName;
                    Type type = null;
                    foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
                    {
                        if (t.AssemblyQualifiedName.Contains(SearchMethodName))
                        {
                            type = t;
                            break;
                        }
                    }
                    retRunner = Activator.CreateInstance(type) as IDirectMethodRunner;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"GetRunner failed.({ex})");
            }

            return retRunner;
        }
    }
}
