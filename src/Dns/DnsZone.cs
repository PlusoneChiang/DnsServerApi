﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Dns
{
    /// <summary>
    /// DNS Zone
    /// </summary>
    public abstract class DnsZone : IDisposable
    {
        /// <summary>
        /// Associated DNS Server
        /// </summary>
        public DnsServer Server { get; internal set; }
        /// <summary>
        /// Zone Domain Name
        /// </summary>
        public string DomainName { get; }

        /// <summary>
        /// Zone Start of Authority Record
        /// </summary>
        public virtual DnsSOARecord StartOfAuthority => GetRecords(DnsRecordTypes.SOA).Cast<DnsSOARecord>().First();

        /// <summary>
        /// Zone Name Server Records
        /// </summary>
        public virtual IEnumerable<DnsNSRecord> NameServers => GetRecords(DnsRecordTypes.NS).Cast<DnsNSRecord>();

        /// <summary>
        /// List of DNS records
        /// </summary>
        public abstract IEnumerable<DnsRecord> Records { get; }

        /// <summary>
        /// Constructs a new instance of <see cref="DnsZone"/>.
        /// </summary>
        /// <param name="server">Associated DNS Server</param>
        /// <param name="domainName">Zone Domain Name</param>
        public DnsZone(DnsServer server, string domainName)
        {
            if (string.IsNullOrWhiteSpace(domainName))
                throw new ArgumentNullException(nameof(domainName));

            Server = server;
            DomainName = domainName;
        }

        /// <summary>
        /// Constructs a new instance of <see cref="DnsZone"/>.
        /// </summary>
        /// <param name="domainName">Zone Domain Name</param>
        public DnsZone(string domainName)
            : this(server: null, domainName)
        {
        }

        /// <summary>
        /// Deletes this zone from the DNS server
        /// </summary>
        public void Delete()
        {
            if (Server == null)
                throw new InvalidOperationException("A zone must be associated with a server to be deleted");

            Server.DeleteZone(this);
        }

        /// <summary>
        /// Retrieves DNS records by record type
        /// </summary>
        /// <param name="recordType">Type of records to be retrieved</param>
        /// <returns>List of DNS records</returns>
        public virtual IEnumerable<DnsRecord> GetRecords(DnsRecordTypes recordType)
            => Records.Where(r => r.Type == recordType);
        /// <summary>
        /// Retrieves DNS records by record type and name
        /// </summary>
        /// <param name="recordType">Type of records to be retrieved</param>
        /// <param name="name">Owner name</param>
        /// <returns>List of DNS records</returns>
        public virtual IEnumerable<DnsRecord> GetRecords(DnsRecordTypes recordType, string name)
            => Records.Where(r => r.Type == recordType && string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));
        /// <summary>
        /// Retrieves DNS records by name
        /// </summary>
        /// <param name="name">Owner name</param>
        /// <returns>List of DNS records</returns>
        public virtual IEnumerable<DnsRecord> GetRecords(string name)
            => Records.Where(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Adds a DNS record to the zone
        /// </summary>
        /// <param name="recordTemplate">Template record to be added</param>
        /// <param name="record">Resulting created record</param>
        /// <returns>True if the record was created, otherwise false</returns>
        public virtual bool TryCreateRecord(DnsRecord recordTemplate, out DnsRecord record)
        {
            if (recordTemplate == null)
            {
                record = null;
                return false;
            }

            try
            {
                record = CreateRecord(recordTemplate);
                return true;
            }
            catch (Exception)
            {
                record = null;
                return false;
            }
        }
        /// <summary>
        /// Adds a DNS record to the zone
        /// </summary>
        /// <param name="recordTemplate">Template record to be added</param>
        /// <returns>Resulting created record</returns>
        public abstract DnsRecord CreateRecord(DnsRecord recordTemplate);

        /// <summary>
        /// Updates a DNS record
        /// </summary>
        /// <param name="record">Record to be updated</param>
        /// <returns>True if the record was updated, otherwise false</returns>
        public virtual bool TrySaveRecord(DnsRecord record)
        {
            if (record == null)
                return false;

            try
            {
                SaveRecord(record);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// Updates a DNS record
        /// </summary>
        /// <param name="record">True if the record was updated, otherwise false</param>
        public abstract void SaveRecord(DnsRecord record);

        /// <summary>
        /// Removes a DNS record from the zone
        /// </summary>
        /// <param name="record">Record to be removed</param>
        /// <returns>True if the record was removed, otherwise false</returns>
        public virtual bool TryDeleteRecord(DnsRecord record)
        {
            if (record == null)
                return false;

            try
            {
                DeleteRecord(record);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// Removes a DNS record from the zone
        /// </summary>
        /// <param name="record">Record to be removed</param>
        public abstract void DeleteRecord(DnsRecord record);

        /// <summary>
        /// Searches for DNS records
        /// </summary>
        /// <param name="query">Text query to search</param>
        /// <returns>List of records matching the query</returns>
        public virtual IEnumerable<DnsRecord> SearchRecords(string query)
            => Records.Where(r => r.ToString().IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0);
        /// <summary>
        /// Searches for DNS records of a given type
        /// </summary>
        /// <param name="recordType">Record type to restrict the search</param>
        /// <param name="query">Text query to search</param>
        /// <returns>List of records matching the query</returns>
        public virtual IEnumerable<DnsRecord> SearchRecords(DnsRecordTypes recordType, string query)
            => Records.Where(r => r.Type == recordType && r.ToString().IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0);

        /// <summary>
        /// Returns a textual representation of the current instance
        /// </summary>
        /// <returns>A textual representation of the current instance</returns>
        public override string ToString()
            => $"DNS Zone [{DomainName}]";

        /// <summary>
        /// Releases all resources used by the current instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Releases all resources used by the current instance.
        /// </summary>
        /// <param name="disposing">True if disposing, false if finalising</param>
        protected virtual void Dispose(bool disposing)
        {
        }
        /// <summary>
        /// Finalise
        /// </summary>
        ~DnsZone()
        {
            Dispose(false);
        }
    }
}
