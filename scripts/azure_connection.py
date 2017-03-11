'''
Borrowed heavily from:
https://github.com/Azure/azure-documentdb-python/blob/master/samples/CollectionManagement/Program.py
'''
import pydocumentdb.documents as documents
import pydocumentdb.document_client as document_client
HOST = 'https://trashtalk.documents.azure.com:443/'
MASTER_KEY = '990PgD69qehN92Jt1Coq1KUeLXhOSgs5RUaGUCxx5IuuMYEzpR9Yh5yl3thLqcLw25Xb5lpbPsps5Qu2k3BdHA=='
DATABASE_ID = 'trashTalk'
COLLECTION_ID = 'trashTalkStatus'
database_link = 'dbs/{}'.format(DATABASE_ID)


class IDisposable:
    """ A context manager to automatically close an object with a close method
    in a with statement. """

    def __init__(self, obj):
        self.obj = obj

    def __enter__(self):
        return self.obj # bound to target

    def __exit__(self, exception_type, exception_val, trace):
        # extra cleanup in here
        self = None


def fetch_azure_data():
    with IDisposable(document_client.DocumentClient(HOST, {'masterKey': MASTER_KEY})) as client:
        collection_link = database_link + '/colls/{0}'.format(COLLECTION_ID)

        trash_cans = list(client.QueryDocuments(collection_link, 'SELECT * FROM c '))

        data = []
        for can in trash_cans:
            data.extend(can['TrashCanStatuses'])

    return data



