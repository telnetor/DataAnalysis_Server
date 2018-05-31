using System;
using System.Collections.Generic;


namespace DataAnalysis.Component.Tools.Cache
{
    /// <summary>
    /// ����ӿ�
    /// </summary>
    public interface ICacheManager
    {

        /// <summary>
        /// �������ʱ��
        /// </summary>
        int TimeOut { set; get; }
        /// <summary>
        /// ���ָ�����Ļ���ֵ
        /// </summary>
        /// <param name="key">�����</param>
        /// <returns>����ֵ</returns>
        object Get(string key);

        /// <summary>
        ///�������� RedisKey ��ǰ׺
        /// </summary>
        string PrefixKey { get; set; }

        /// <summary>
        /// ���ָ�����Ļ���ֵ
        /// </summary>
        T Get<T>(string key);
        /// <summary>
        /// �ӻ������Ƴ�ָ�����Ļ���ֵ
        /// </summary>
        /// <param name="key">�����</param>
        void Remove(string key);
        /// <summary>
        /// ������л������
        /// </summary>
        //void Clear();
        /// <summary>
        /// ��ָ�����Ķ�����ӵ�������
        /// </summary>
        /// <param name="key">�����</param>
        /// <param name="data">����ֵ</param>
        void Insert(string key, object data);
        /// <summary>
        /// ��ָ�����Ķ�����ӵ�������
        /// </summary>
        /// <param name="key">�����</param>
        /// <param name="data">����ֵ</param>
        void Insert<T>(string key, T data);
        /// <summary>
        /// ��ָ�����Ķ�����ӵ������У���ָ������ʱ��
        /// </summary>
        /// <param name="key">�����</param>
        /// <param name="data">����ֵ</param>
        /// <param name="cacheTime">�������ʱ��(����)</param>
        void Insert(string key, object data, int cacheTime);

        /// <summary>
        /// ��ָ�����Ķ�����ӵ������У���ָ������ʱ��
        /// </summary>
        /// <param name="key">�����</param>
        /// <param name="data">����ֵ</param>
        /// <param name="cacheTime">�������ʱ��(����)</param>
        void Insert<T>(string key, T data, int cacheTime);
        /// <summary>
        /// ��ָ�����Ķ�����ӵ������У���ָ������ʱ��
        /// </summary>
        /// <param name="key">�����</param>
        /// <param name="data">����ֵ</param>
        /// <param name="cacheTime">�������ʱ��</param>
        void Insert(string key, object data, DateTime cacheTime);
        /// <summary>
        /// ��ָ�����Ķ�����ӵ������У���ָ������ʱ��
        /// </summary>
        /// <param name="key">�����</param>
        /// <param name="data">����ֵ</param>
        /// <param name="cacheTime">�������ʱ��</param>
        void Insert<T>(string key, T data, DateTime cacheTime);
        /// <summary>
        /// �ж�key�Ƿ����
        /// </summary>
        bool Exists(string key);

        /// <summary>
        /// ��ȡ���е�KEY
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        List<string> GetKeys(string pattern="");

        /// <summary>
        /// ��� Key ��ǰ׺
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string AddKeyPrefix(string key);

        /// <summary>
        /// ����ɾ��
        /// </summary>
        /// <param name="pattern"></param>
        void RemoveBatch(string pattern = "");

    }
}
