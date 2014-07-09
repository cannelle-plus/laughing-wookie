var emitReadModel = function (s, e) {
    var streamId = "GameFlatReadModel-" + e.streamId.replace("Game-", "");
    var eventType = e.eventType + "_GameFlatReadModel";
    emit(streamId, eventType, s);
};
fromCategory('Game').foreachStream().when({
    $init: function () {
        return {
            name: null,
            location: null,
            date: null,
            nbPlayer: 0
        };
    },
    "GameCreated": function (s, e) {
        s.name = e.body.value;
          emitReadModel(s, e);
    },
    "GameJoined": function (s, e) {
        s.nbPlayer += 1;
        emitReadModel(s, e);
    },
    "GameAbandonned": function (s, e) {
        s.nbPlayer -= 1;
        emitReadModel(s, e);
    }
});