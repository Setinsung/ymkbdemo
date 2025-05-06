// 封装对浏览器 IndexedDB 的操作
window.indexedDbStorage = {
    // Open the database (creates it if it doesn't exist, or upgrades if needed)
    open: function (dbName = "AppDb") {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(dbName, 3); // Increment version to 3

            // Handle database upgrades
            request.onupgradeneeded = function (event) {
                const db = event.target.result;

                let cacheStore;

                // Create 'cache' store if it does not exist
                if (!db.objectStoreNames.contains('cache')) {
                    cacheStore = db.createObjectStore('cache', {keyPath: 'key'});
                } else {
                    cacheStore = event.currentTarget.transaction.objectStore('cache');
                }

                // Ensure 'tags' index exists
                if (!cacheStore.indexNames.contains('tags')) {
                    cacheStore.createIndex('tags', 'tags', {multiEntry: true});
                }
            };

            request.onsuccess = function (event) {
                resolve(event.target.result); // Resolve with the database instance
            };

            request.onerror = function (event) {
                reject(event.target.error); // Reject with the error
            };
        });
    },

    // Save data to the cache store (with JSON serialization and optional tags)
    saveData: function (dbName, key, value, tags = [], expiration = null) {
        return this.open(dbName).then(db => {
            return new Promise((resolve, reject) => {
                const transaction = db.transaction('cache', 'readwrite');
                const store = transaction.objectStore('cache');
                const serializedValue = JSON.stringify(value); // Serialize the value

                const request = store.put({
                    key: key,
                    value: serializedValue,
                    tags: tags,
                    expiration: expiration ? new Date(Date.now() + expiration).toISOString() : null // Add expiration
                }); // Include tags

                request.onsuccess = () => resolve();
                request.onerror = () => reject(request.error);
            });
        });
    },

    // Get all data by tags (array of tags)
    getDataByTags: function (dbName, tags) {
        return this.open(dbName).then(db => {
            return new Promise((resolve, reject) => {
                const transaction = db.transaction('cache', 'readonly'); // Open a readonly transaction on 'cache'
                const store = transaction.objectStore('cache');         // Access the 'cache' object store
                const index = store.index('tags');                     // Access the 'tags' index

                const results = [];
                const deletePromises = [];
                // Fetch data for each tag
                const tagRequests = tags.map(tag => {
                    return new Promise((tagResolve, tagReject) => {
                        const request = index.getAll(tag); // Retrieve all entries matching the tag

                        request.onsuccess = function (event) {
                            const entries = event.target.result;
                            // Push key and deserialized value into the results array
                            entries.forEach(entry => {
                                // Check expiration
                                if (!entry.expiration || new Date(entry.expiration) >= new Date()) {
                                    results.push({key: entry.key, value: JSON.parse(entry.value)});
                                } else {
                                    // Add deletion of expired data
                                    deletePromises.push(this.deleteData(dbName, entry.key));
                                }
                            });
                            tagResolve();
                        };

                        request.onerror = function (event) {
                            tagReject(event.target.error); // Handle errors
                        };
                    });
                });

                // Combine results for all tags
                Promise.all(tagRequests)
                    .then(() => Promise.all(deletePromises)) // Ensure expired data is deleted
                    .then(() => resolve(results)) // Return the list of { key, value }
                    .catch(reject);
            });
        });
    },

    // Delete all data with specific tags (array of tags)
    deleteDataByTags: function (dbName, tags) {
        return this.open(dbName).then(db => {
            return new Promise((resolve, reject) => {
                const transaction = db.transaction('cache', 'readwrite');
                const store = transaction.objectStore('cache');
                const index = store.index('tags');

                const deletePromises = [];
                const tagRequests = tags.map(tag => {
                    return new Promise((tagResolve, tagReject) => {
                        const request = index.getAllKeys(tag);

                        request.onsuccess = function (event) {
                            const keys = event.target.result;
                            const keyDeletes = keys.map(key => {
                                return new Promise((delResolve, delReject) => {
                                    const deleteRequest = store.delete(key);
                                    deleteRequest.onsuccess = () => delResolve();
                                    deleteRequest.onerror = () => delReject(deleteRequest.error);
                                });
                            });
                            deletePromises.push(...keyDeletes);
                            tagResolve();
                        };

                        request.onerror = function (event) {
                            tagReject(event.target.error);
                        };
                    });
                });

                Promise.all(tagRequests)
                    .then(() => Promise.all(deletePromises))
                    .then(resolve)
                    .catch(reject);
            });
        });
    },

    // Get data from the cache store (with JSON deserialization)
    getData: function (dbName, key) {
        return this.open(dbName).then(db => {
            return new Promise((resolve, reject) => {
                const transaction = db.transaction('cache', 'readonly');
                const store = transaction.objectStore('cache');
                const request = store.get(key);

                request.onsuccess = function (event) {
                    const result = event.target.result;
                    if (result) {
                        // Check expiration
                        if (result.expiration && new Date(result.expiration) < new Date()) {
                            window.indexedDbStorage.deleteData(dbName, key).then(() => resolve(null)) // Delete expired data
                                .catch(reject);
                        } else {
                            try {
                                resolve(JSON.parse(result.value)); // Deserialize the value
                            } catch (e) {
                                reject("Error parsing JSON");
                            }
                        }
                    } else {
                        resolve(null); // No data found
                    }
                };

                request.onerror = function (event) {
                    reject(event.target.error);
                };
            });
        });
    },

    // Clear all data from the cache store
    clearData: function (dbName) {
        return this.open(dbName).then(db => {
            return new Promise((resolve, reject) => {
                const transaction = db.transaction('cache', 'readwrite');
                const store = transaction.objectStore('cache');
                const request = store.clear(); // Clear all data in the store

                request.onsuccess = function () {
                    resolve(); // Success, data cleared
                };

                request.onerror = function (event) {
                    reject(event.target.error); // Error clearing data
                };
            });
        });
    },

    // Delete a specific key from the cache store
    deleteData: function (dbName, key) {
        return this.open(dbName).then(db => {
            return new Promise((resolve, reject) => {
                const transaction = db.transaction('cache', 'readwrite');
                const store = transaction.objectStore('cache');
                const request = store.delete(key); // Delete the specified key

                request.onsuccess = function () {
                    resolve(); // Success, key deleted
                };

                request.onerror = function (event) {
                    reject(event.target.error); // Error deleting key
                };
            });
        });
    }
};
