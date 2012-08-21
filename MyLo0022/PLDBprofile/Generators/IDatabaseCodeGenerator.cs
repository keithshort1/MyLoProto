﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using Microsoft.VisualStudio.Uml.AuxiliaryConstructs;
using Microsoft.VisualStudio.Uml.Classes;
using System.ComponentModel.Composition;


namespace Generators
{
    public interface IDatabaseCodeGenerator
    {
        void AddFKConstraint(String constraintName, String className, String sourceColName, String targetTableName, String targetColumn, String deleteAction);

        void AddProperty(IProperty p);

        void AddPKProperty(IProperty p);

        void AddFKProperty(IProperty ae);

        void AddAbstractReferenceProperty(IProperty ae);

        void AddReferenceProperty(IProperty ae);

        void AddCompositeProperty(IProperty p);

        void AddStreamWriter(StreamWriter s);

        void WriteHeader();

        void WriteTrailer();

        void WriteStructureFinish(IClass cls);

        void WriteStructureStart(IClass cls);

        void WriteStructureBody(IClass cls, string pkName);

        void Initialize();

    }

    public interface IDatabaseTableGenerator  : IDatabaseCodeGenerator
    {
        void AddIndexConstraint(string indexName, string columns, string indexType);

        void AddUniqueConstraint(List<string> columns);

        void AddSubtreeProcedure(string hierachyTableName, string kindName, string parentName, string childName);

        void WriteSubtreeProcedure();

        void WriteAlterTables();

        void WriteSecureView(IClass cls, string pkName);
    }
}
