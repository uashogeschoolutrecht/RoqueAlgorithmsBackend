mod domain;

use postgres::{Client, NoTls};
use domain::{Node, Similarity, Link, Graph};
use serde_json::to_string;
use std::{fs::write, borrow::Borrow};


fn main() {
    let mut client = Client::connect("host=localhost dbname=RogueDB user=FakenewsAdmin password=olufsen", NoTls).unwrap();
    let mut all_nodes: Vec<Node> = Vec::new();
    let mut all_similarities: Vec<Similarity>= Vec::new();
    let mut all_links: Vec<Link> = Vec::new();

    for row in client.query("select (Id, Url, NumberOfArticles, NumberOfArticlesScraped ) from Websites", &[]).unwrap() {
        let mut node = Node{
            id:row.get(0),
            label: row.get(1).to_owned(),
            total_articles: row.get(2),
            total_articles_scraped: row.get(3),
            amount_of_connections : 0
        };
        all_nodes.push(node);
    };
    for row in client.query("select (OriginalWebsiteId, FoundWebsiteId,
        UrlToOriginalArticle, UrlToFoundArticle, SimilarityScore, 
        OriginalLanguage,FoundLanguage, OriginalPostDate, FoundPostDate
    ) from Similarities where SimilarityScore > 0.7", &[]).unwrap() {
        let sim = Similarity{
            original_id: row.get(0),
            found_id: row.get(1),
            original_url: row.get(2),
            found_url: row.get(3),
            score: row.get(4),
            original_date: row.get(5),
            found_date: row.get(6),
            original_lan: row.get(7),
            found_lan: row.get(8)
        };
        all_similarities.push(sim);
    }

    for(i, node1) in all_nodes.into_iter().enumerate() {
        for node2 in all_nodes.iter() {
            let mut node1 = all_nodes[i];
            println!("node1: {} - node 2: {}", node1.id, node2.id);
            if node1.id == node2.id{
                continue;
            }
            let links_between_nodes: Vec<Similarity> = all_similarities.clone().into_iter()
                .filter(|sim| sim.original_id == node1.id && sim.found_id == node2.id)
                .collect::<Vec<Similarity>>();

            if links_between_nodes.len() == 0{
                continue;
            }
            
            node1.amount_of_connections += 1;
            node2.amount_of_connections += 1;
            let link = Link {
                source: node1.id,
                target: node2.id,
                amount_of_copies: links_between_nodes.len(),
                similarities: links_between_nodes
            };
            all_links.push(link);   
            
        }
    }
    all_nodes = all_nodes.into_iter()
        .filter(|node| node.amount_of_connections > 0)
        .collect::<Vec<Node>>();

    let graph = Graph {nodes: all_nodes, links: all_links};
    let json_string = to_string(&graph).unwrap();
    write("data.json", json_string).unwrap();
}
