using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UML2Netbeans
{
    class Package
    {
        public String[] getPackage(String projectRootFolder)
        {
            String[] allPackages = null;
            try
            {
                allPackages = Directory.GetDirectories(projectRootFolder + "\\src");

                for (int i = 0; i < allPackages.Length; i++)
                {
                    Boolean hasStillSubFolders = true;
                    while (hasStillSubFolders)
                    {
                        if (Directory.GetDirectories(allPackages[i]).Length > 0)
                        {
                            //if this condition is true-->> Directory has sub-directories
                            allPackages[i] = Directory.GetDirectories(allPackages[i])[0];


                        }
                        else
                        {
                            hasStillSubFolders = false;
                            break;
                        }
                    }

                }

            }
            catch
            {
                Console.WriteLine("Please select a Netbeans Project first");
            }
            return allPackages;
        }
    }
}
