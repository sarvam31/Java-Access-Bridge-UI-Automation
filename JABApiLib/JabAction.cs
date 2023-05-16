using System;
using System.Collections.Generic;
using System.Diagnostics;
using static JABApiLib.JabHelpers;
using static JABApiLib.JabApi;

namespace JABApiLib
{
    
    public class JabAction
    {
        public static string elementName; 
        public static HashSet<String> elementType = new HashSet<string> {};

        /*Api exposed to developer for performing action on Java Application elements.
         Parameters:
            Input:
                window: Title of window which contains UI element (Refer Access Bridge Explorer for title)
                element: Text of element to perform action on. (should be same string as given in Access Bridge Explorer, Name property
                action: Action to perform on UI Element.
                    Possible options are : 
                        1> click     2> setText    3> element_exists 	
                text (Optional): Text to be set in case of setText action.
                timeout: implicit wait in seconds for element to be found
            Output:
                Returns true/false for action complete status
        */
        public Boolean UIAction(string window, string element, string action, string text = "", int timeout = 30)
        {
            

            JabAction.elementName = element;
            Boolean complete = false;

            while ((timeout--) != 0 && complete == false)
            {
                try
                {
                    Init();

                    int vmID = 0;
                    AccessibleTreeItem javaTree = null;
                    AccessibleTreeItem UIelement = null;

                    //fetching UI tree
                    javaTree = GetComponentTreeByTitle(window, out vmID);
                    

                    //Debug.WriteLine("return value: " + UIelement.name.ToString() + "\treturn role: " + UIelement.role + "\tstates: " + UIelement.states_en_US);
                    /*
                    List<String> actionList = GetAccessibleActionsList(vmID, UIelement.acPtr);
                    Debug.WriteLine("Actions:");
                    foreach(var act in actionList)
                    {
                        Debug.WriteLine(act);
                    }*/

                    //performing UI Action
                    switch (action)
                    {
                        case "click":
                        case "クリック":
                            JabAction.elementType.Add("radio button");
                            JabAction.elementType.Add("push button");
                            UIelement = findElement(javaTree, 0);
                            complete = DoAccessibleActions(vmID, UIelement.acPtr, action);
                            break;
                        case "setText":
                            JabAction.elementType.Add("text");
                            JabAction.elementType.Add("password text");
                            UIelement = findElement(javaTree, 0);
                            complete = setTextContents(vmID, UIelement.acPtr, text);
                            break;
                        case "element_exists":
                            JabAction.elementType.Add("label");
                            JabAction.elementType.Add("radio button");
                            JabAction.elementType.Add("push button");
                            JabAction.elementType.Add("text");
                            JabAction.elementType.Add("password text");
                            UIelement = findElement(javaTree, 0);
                            complete = UIelement.name.ToString().Equals(element);
                            break;
                        default:
                            complete = false;
                            System.Threading.Thread.Sleep(1000);
                            break;
                    }

                }
                catch (Exception)
                {
                    System.Threading.Thread.Sleep(1000);
                    //TODO: Check any memory leak due to new instances of Java Access Bridges launched while waiting for element to exist.
                }
            }

            return complete;

        }

        /*Finding required UI element through recursive calls
         Parameters:
            Input:
                element: element in UI Tree of application, pass root node as element to initiate function call for element search
                level: level of element in UI Tree, used for debugging purpose, no functional utility yet
            Output:
                Return UIElement if found, else null
        */
        private AccessibleTreeItem findElement(AccessibleTreeItem element, int level)
        {
            //Debug.WriteLine("\n=>Name: " + element.name + "\t Level: " + level);
            if (element == null)
                return null;
            
            if (element.name.Equals(JabAction.elementName) &&
                JabAction.elementType.Contains(element.role_en_US) &&
                element.states_en_US.Contains("enabled")
                )
            {
                return element;
            }

            List<AccessibleTreeItem> children = new List<AccessibleTreeItem>();

            //checking children nodes
            for (int idx = 0; idx < element.children.Count; idx++)
            {
                children.Add(findElement(element.children[idx], level + 1));
            }

            foreach (var item in children)
            {
                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }

        
    }

    

    
}
