﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QLDHCDAPI.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class QLDHCDEntities : DbContext
    {
        public QLDHCDEntities()
            : base("name=QLDHCDEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BANGYKIEN> BANGYKIENs { get; set; }
        public virtual DbSet<BANGYKIENBQPHIEU> BANGYKIENBQPHIEUx { get; set; }
        public virtual DbSet<CODONG> CODONGs { get; set; }
        public virtual DbSet<CT_DHCD> CT_DHCD { get; set; }
        public virtual DbSet<CT_YKIEN_BQPHIEU> CT_YKIEN_BQPHIEU { get; set; }
        public virtual DbSet<CTBQYKIEN> CTBQYKIENs { get; set; }
        public virtual DbSet<DHCD> DHCDs { get; set; }
        public virtual DbSet<DHCDSTT> DHCDSTTs { get; set; }
        public virtual DbSet<EXCEPTIONAPP> EXCEPTIONAPPs { get; set; }
        public virtual DbSet<USERCD> USERCDs { get; set; }
        public virtual DbSet<UYQUYEN> UYQUYENs { get; set; }
        public virtual DbSet<COMMITDHCD> COMMITDHCDs { get; set; }
        public virtual DbSet<THANHVIENBK> THANHVIENBKS { get; set; }
        public virtual DbSet<THANHVIENHDQT> THANHVIENHDQTs { get; set; }
        public virtual DbSet<CT_BAUBKS> CT_BAUBKS { get; set; }
        public virtual DbSet<CT_BAUHDQT> CT_BAUHDQT { get; set; }
    
        public virtual int CT_BAUBKS_INSERT(string mABKS, string mANGUOIBAU, Nullable<int> sLPHIEUBAU, string hINHTHUCBAU, Nullable<bool> lAHOPLE, ObjectParameter cHECKSUCCESS)
        {
            var mABKSParameter = mABKS != null ?
                new ObjectParameter("MABKS", mABKS) :
                new ObjectParameter("MABKS", typeof(string));
    
            var mANGUOIBAUParameter = mANGUOIBAU != null ?
                new ObjectParameter("MANGUOIBAU", mANGUOIBAU) :
                new ObjectParameter("MANGUOIBAU", typeof(string));
    
            var sLPHIEUBAUParameter = sLPHIEUBAU.HasValue ?
                new ObjectParameter("SLPHIEUBAU", sLPHIEUBAU) :
                new ObjectParameter("SLPHIEUBAU", typeof(int));
    
            var hINHTHUCBAUParameter = hINHTHUCBAU != null ?
                new ObjectParameter("HINHTHUCBAU", hINHTHUCBAU) :
                new ObjectParameter("HINHTHUCBAU", typeof(string));
    
            var lAHOPLEParameter = lAHOPLE.HasValue ?
                new ObjectParameter("LAHOPLE", lAHOPLE) :
                new ObjectParameter("LAHOPLE", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CT_BAUBKS_INSERT", mABKSParameter, mANGUOIBAUParameter, sLPHIEUBAUParameter, hINHTHUCBAUParameter, lAHOPLEParameter, cHECKSUCCESS);
        }
    
        public virtual int CT_BAUBKS_INSERT_CURRENTUSER(string mABKS, string uSERNAME, Nullable<int> sLPHIEUBAU, string hINHTHUCBAU, Nullable<bool> lAHOPLE, ObjectParameter cHECKSUCCESS)
        {
            var mABKSParameter = mABKS != null ?
                new ObjectParameter("MABKS", mABKS) :
                new ObjectParameter("MABKS", typeof(string));
    
            var uSERNAMEParameter = uSERNAME != null ?
                new ObjectParameter("USERNAME", uSERNAME) :
                new ObjectParameter("USERNAME", typeof(string));
    
            var sLPHIEUBAUParameter = sLPHIEUBAU.HasValue ?
                new ObjectParameter("SLPHIEUBAU", sLPHIEUBAU) :
                new ObjectParameter("SLPHIEUBAU", typeof(int));
    
            var hINHTHUCBAUParameter = hINHTHUCBAU != null ?
                new ObjectParameter("HINHTHUCBAU", hINHTHUCBAU) :
                new ObjectParameter("HINHTHUCBAU", typeof(string));
    
            var lAHOPLEParameter = lAHOPLE.HasValue ?
                new ObjectParameter("LAHOPLE", lAHOPLE) :
                new ObjectParameter("LAHOPLE", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CT_BAUBKS_INSERT_CURRENTUSER", mABKSParameter, uSERNAMEParameter, sLPHIEUBAUParameter, hINHTHUCBAUParameter, lAHOPLEParameter, cHECKSUCCESS);
        }
    
        public virtual int CT_BAUHDQT_INSERT(string mAHDQT, string mANGUOIBAU, Nullable<int> sLPHIEUBAU, string hINHTHUCBAU, Nullable<bool> lAHOPLE, ObjectParameter cHECKSUCCESS)
        {
            var mAHDQTParameter = mAHDQT != null ?
                new ObjectParameter("MAHDQT", mAHDQT) :
                new ObjectParameter("MAHDQT", typeof(string));
    
            var mANGUOIBAUParameter = mANGUOIBAU != null ?
                new ObjectParameter("MANGUOIBAU", mANGUOIBAU) :
                new ObjectParameter("MANGUOIBAU", typeof(string));
    
            var sLPHIEUBAUParameter = sLPHIEUBAU.HasValue ?
                new ObjectParameter("SLPHIEUBAU", sLPHIEUBAU) :
                new ObjectParameter("SLPHIEUBAU", typeof(int));
    
            var hINHTHUCBAUParameter = hINHTHUCBAU != null ?
                new ObjectParameter("HINHTHUCBAU", hINHTHUCBAU) :
                new ObjectParameter("HINHTHUCBAU", typeof(string));
    
            var lAHOPLEParameter = lAHOPLE.HasValue ?
                new ObjectParameter("LAHOPLE", lAHOPLE) :
                new ObjectParameter("LAHOPLE", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CT_BAUHDQT_INSERT", mAHDQTParameter, mANGUOIBAUParameter, sLPHIEUBAUParameter, hINHTHUCBAUParameter, lAHOPLEParameter, cHECKSUCCESS);
        }
    
        public virtual int CT_BAUHDQT_INSERT_CURRENTUSER(string mAHDQT, string uSERNAME, Nullable<int> sLPHIEUBAU, string hINHTHUCBAU, Nullable<bool> lAHOPLE, ObjectParameter cHECKSUCCESS)
        {
            var mAHDQTParameter = mAHDQT != null ?
                new ObjectParameter("MAHDQT", mAHDQT) :
                new ObjectParameter("MAHDQT", typeof(string));
    
            var uSERNAMEParameter = uSERNAME != null ?
                new ObjectParameter("USERNAME", uSERNAME) :
                new ObjectParameter("USERNAME", typeof(string));
    
            var sLPHIEUBAUParameter = sLPHIEUBAU.HasValue ?
                new ObjectParameter("SLPHIEUBAU", sLPHIEUBAU) :
                new ObjectParameter("SLPHIEUBAU", typeof(int));
    
            var hINHTHUCBAUParameter = hINHTHUCBAU != null ?
                new ObjectParameter("HINHTHUCBAU", hINHTHUCBAU) :
                new ObjectParameter("HINHTHUCBAU", typeof(string));
    
            var lAHOPLEParameter = lAHOPLE.HasValue ?
                new ObjectParameter("LAHOPLE", lAHOPLE) :
                new ObjectParameter("LAHOPLE", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CT_BAUHDQT_INSERT_CURRENTUSER", mAHDQTParameter, uSERNAMEParameter, sLPHIEUBAUParameter, hINHTHUCBAUParameter, lAHOPLEParameter, cHECKSUCCESS);
        }
    
        public virtual int UYQUYEN_INSERT(string mANGCHUYEN, string mANGNHAN, string uYQUYENTYPE, Nullable<int> sLUQ, string hoTen, Nullable<int> cMND, Nullable<System.DateTime> ngayCap, string noiCap, string diaChi, string quocTich, ObjectParameter cHECKSUCCESS)
        {
            var mANGCHUYENParameter = mANGCHUYEN != null ?
                new ObjectParameter("MANGCHUYEN", mANGCHUYEN) :
                new ObjectParameter("MANGCHUYEN", typeof(string));
    
            var mANGNHANParameter = mANGNHAN != null ?
                new ObjectParameter("MANGNHAN", mANGNHAN) :
                new ObjectParameter("MANGNHAN", typeof(string));
    
            var uYQUYENTYPEParameter = uYQUYENTYPE != null ?
                new ObjectParameter("UYQUYENTYPE", uYQUYENTYPE) :
                new ObjectParameter("UYQUYENTYPE", typeof(string));
    
            var sLUQParameter = sLUQ.HasValue ?
                new ObjectParameter("SLUQ", sLUQ) :
                new ObjectParameter("SLUQ", typeof(int));
    
            var hoTenParameter = hoTen != null ?
                new ObjectParameter("HoTen", hoTen) :
                new ObjectParameter("HoTen", typeof(string));
    
            var cMNDParameter = cMND.HasValue ?
                new ObjectParameter("CMND", cMND) :
                new ObjectParameter("CMND", typeof(int));
    
            var ngayCapParameter = ngayCap.HasValue ?
                new ObjectParameter("NgayCap", ngayCap) :
                new ObjectParameter("NgayCap", typeof(System.DateTime));
    
            var noiCapParameter = noiCap != null ?
                new ObjectParameter("NoiCap", noiCap) :
                new ObjectParameter("NoiCap", typeof(string));
    
            var diaChiParameter = diaChi != null ?
                new ObjectParameter("DiaChi", diaChi) :
                new ObjectParameter("DiaChi", typeof(string));
    
            var quocTichParameter = quocTich != null ?
                new ObjectParameter("QuocTich", quocTich) :
                new ObjectParameter("QuocTich", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UYQUYEN_INSERT", mANGCHUYENParameter, mANGNHANParameter, uYQUYENTYPEParameter, sLUQParameter, hoTenParameter, cMNDParameter, ngayCapParameter, noiCapParameter, diaChiParameter, quocTichParameter, cHECKSUCCESS);
        }
    
        public virtual int UYQUYEN_INSERT_BYUSERNAME(string uSERNAME, string mANGNHAN, string uYQUYENTYPE, Nullable<int> sLUQ, string hoTen, Nullable<int> cMND, Nullable<System.DateTime> ngayCap, string noiCap, string diaChi, string quocTich, ObjectParameter cHECKSUCCESS)
        {
            var uSERNAMEParameter = uSERNAME != null ?
                new ObjectParameter("USERNAME", uSERNAME) :
                new ObjectParameter("USERNAME", typeof(string));
    
            var mANGNHANParameter = mANGNHAN != null ?
                new ObjectParameter("MANGNHAN", mANGNHAN) :
                new ObjectParameter("MANGNHAN", typeof(string));
    
            var uYQUYENTYPEParameter = uYQUYENTYPE != null ?
                new ObjectParameter("UYQUYENTYPE", uYQUYENTYPE) :
                new ObjectParameter("UYQUYENTYPE", typeof(string));
    
            var sLUQParameter = sLUQ.HasValue ?
                new ObjectParameter("SLUQ", sLUQ) :
                new ObjectParameter("SLUQ", typeof(int));
    
            var hoTenParameter = hoTen != null ?
                new ObjectParameter("HoTen", hoTen) :
                new ObjectParameter("HoTen", typeof(string));
    
            var cMNDParameter = cMND.HasValue ?
                new ObjectParameter("CMND", cMND) :
                new ObjectParameter("CMND", typeof(int));
    
            var ngayCapParameter = ngayCap.HasValue ?
                new ObjectParameter("NgayCap", ngayCap) :
                new ObjectParameter("NgayCap", typeof(System.DateTime));
    
            var noiCapParameter = noiCap != null ?
                new ObjectParameter("NoiCap", noiCap) :
                new ObjectParameter("NoiCap", typeof(string));
    
            var diaChiParameter = diaChi != null ?
                new ObjectParameter("DiaChi", diaChi) :
                new ObjectParameter("DiaChi", typeof(string));
    
            var quocTichParameter = quocTich != null ?
                new ObjectParameter("QuocTich", quocTich) :
                new ObjectParameter("QuocTich", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UYQUYEN_INSERT_BYUSERNAME", uSERNAMEParameter, mANGNHANParameter, uYQUYENTYPEParameter, sLUQParameter, hoTenParameter, cMNDParameter, ngayCapParameter, noiCapParameter, diaChiParameter, quocTichParameter, cHECKSUCCESS);
        }
    
        public virtual int DHCD_CAPNHATNTONG(ObjectParameter cHECKRESULT)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DHCD_CAPNHATNTONG", cHECKRESULT);
        }
    }
}
