﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FalloutRedScare
{
    public class RestrictedBodyPawnKindDef : PawnKindDef
    {
        public List<BodyTypeDef> restrictBodyTypesTo;
    }
}