using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Credits", menuName = "Credits", order = 2)]
public class Credits : ScriptableObject
{
    [Serializable]
    public struct CreditSection
    {
        public string sectionHeader;
        public CreditEntry[] authors;        
    }
    
   [Serializable]
   public struct CreditEntry
    {
        public string author;
        public string website;
    }


    [SerializeField] List<CreditSection> sections;

    public List<CreditSection> GetSections()
    {
        return sections;
    }
}

