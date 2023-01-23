use serde::{Serialize, Deserialize};
 

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct Node<'a> {
    pub id: i32,
    pub label: &'a str,
    pub total_articles: i32,
    pub total_articles_scraped: i32,
    pub amount_of_connections : usize
}
#[derive(Serialize, Deserialize, Debug)]
pub struct Link {
    pub source: i32,
    pub target: i32,
    pub amount_of_copies: usize,
    pub similarities: Vec<Similarity>
}
#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct Similarity {
    pub original_id: i32,
    pub found_id: i32,
    pub original_url: String,
    pub found_url: String,
    pub score: f32,
    pub original_date: String,
    pub found_date: String,
    pub original_lan: String,
    pub found_lan: String
}

#[derive(Serialize)]
pub struct Graph<'a> {
    nodes: Vec<Node<'a>>,
    pub links: Vec<Link>,
}